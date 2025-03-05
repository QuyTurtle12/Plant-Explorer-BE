using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Utils;
using System.Text.Json;

namespace Plant_Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanHistoryController : ControllerBase
    {
        private readonly IPlantService _plantService;
        private readonly IScanHistoryService _scanHistoryService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _plantIdApiKey;
        private readonly string _plantIdIdentifyUrl;
        private readonly string _plantIdRetrieveUrl;

        public ScanHistoryController(IScanHistoryService scanHistoryService, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IConfiguration configuration, IPlantService plantService = null)
        {
            _scanHistoryService = scanHistoryService;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _plantIdApiKey = configuration["PlantId:ApiKey"];
            _plantIdIdentifyUrl = configuration["PlantId:IdentifyUrl"];
            _plantIdRetrieveUrl = configuration["PlantId:RetrieveUrl"];
            _plantService = plantService;
        }

        [HttpPost("identify")]
        public async Task<IActionResult> IdentifyPlant(IFormFile file)
        {
            try
            {
                // Validate the image
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Convert the image to a byte array
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                string base64Image = Convert.ToBase64String(imageBytes);

                // Create the multipart/form-data content
                using var httpClient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _plantIdIdentifyUrl);
                request.Headers.Add("Api-Key", _plantIdApiKey);

                var content = new MultipartFormDataContent();
                content.Add(new StringContent(base64Image), "images");

                request.Content = content;

                var response = await httpClient.SendAsync(request);

                // Handle non-success status codes
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"PlantID API Error: {errorBody}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<PlantIdIdentifyResponse>(responseBody);

                if (result?.AccessToken == null)
                    return BadRequest("Failed to retrieve access token from PlantID.");

                // Cache the image and access token
                string cacheKey = Guid.NewGuid().ToString();
                _memoryCache.Set(cacheKey, new CachedImage
                {
                    ImageBytes = imageBytes,
                    AccessToken = result.AccessToken
                });

                return Ok(new { CacheKey = cacheKey, AccessToken = result.AccessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to identify plant");
            }
        }

        [HttpGet("getPlantInfo/{cacheKey}")]
        public async Task<IActionResult> GetPlantInfo(string cacheKey)
        {
            try
            {
                // Retrieve the cached image and access token
                if (!_memoryCache.TryGetValue(cacheKey, out CachedImage cachedImage))
                    return NotFound("Image not found or cache expired");

                // Step 1: Fetch results from PlantID
                using var httpClient = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, _plantIdRetrieveUrl.Replace("{access_token}", cachedImage.AccessToken));
                request.Headers.Add("Api-Key", _plantIdApiKey);

                var response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error retrieving plant data");

                var responseBody = await response.Content.ReadAsStringAsync();
                var plantResult = JsonSerializer.Deserialize<PlantIdResultsResponse>(responseBody);

                // Extract the scientific name (assuming the first suggestion is the most likely)
                string? scientificName = plantResult?.Result?.Classification?.Suggestions?.FirstOrDefault()?.Name;
                if (string.IsNullOrEmpty(scientificName))
                    return NotFound("Scientific name not found.");

                // Step 2: Get GBIF UsageKey
                string matchUrl = $"https://api.gbif.org/v1/species/match?name={Uri.EscapeDataString(scientificName)}";
                var matchResponse = await httpClient.GetStringAsync(matchUrl);
                var matchResult = JsonSerializer.Deserialize<GbifMatchResponse>(matchResponse);

                if (matchResult?.UsageKey == null)
                    return NotFound("Plant not found in GBIF database.");

                int usageKey = matchResult.UsageKey.Value;

                // Step 3: Fetch species details from GBIF
                string detailsUrl = $"https://api.gbif.org/v1/species/{usageKey}";
                var detailsResponse = await httpClient.GetStringAsync(detailsUrl);
                var detailsResult = JsonSerializer.Deserialize<GbifSpeciesResponse>(detailsResponse);

                if (detailsResult == null)
                    return NotFound("No details found for this plant.");

                // Extract relevant data
                string commonName = detailsResult.VernacularName ?? "Unknown";
                string family = detailsResult.Family ?? "Unknown";

                //Check if plant already exists in the database
                var existingPlant = await _plantService.GetPlantByScientificName(scientificName);

                if (existingPlant == null)
                {
                    // Create a new plant
                    var newPlant = await _plantService.CreatePlantAsync(new PlantPostModel
                    {
                        Name = commonName,
                        ScientificName = scientificName,
                        Family = family,
                        Description = "Description have not implemented yet",
                        Habitat = "Habitat have not implemented yet",
                        Distribution = "Distribution have not implemented yet"
                    });

                    existingPlant = newPlant;
                }

                //Create a new scan history entry
                var newScanHistory = await _scanHistoryService.CreateScanHistoryAsync(new ScanHistoryPostModel
                {
                    //temporarily hardcode user id
                    UserId = Guid.Parse("0E2BE45F-B2C9-4F2F-9879-4AAAAA17BE65"),
                    PlantId = existingPlant.Id,
                    Probability = (decimal)plantResult.Result.Classification.Suggestions.FirstOrDefault().Probability,
                    ImgUrl = "Image have not implemented yet"
                });

                return Ok(new { Plant = existingPlant, ScanHistory = newScanHistory });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to retrieve plant details.");
            }
        }
        

        [HttpGet("getImage/{cacheKey}")]
        public async Task<IActionResult> GetPlantImage(string cacheKey)
        {
            try
            {
                // Retrieve the cached image
                if (!_memoryCache.TryGetValue(cacheKey, out CachedImage cachedImage))
                    return NotFound("Image not found or cache expired");
                return File(cachedImage.ImageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to retrieve plant image");
            }
        }

    }
}

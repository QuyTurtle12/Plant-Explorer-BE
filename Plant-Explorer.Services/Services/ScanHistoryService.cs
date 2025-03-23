using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Utils;
using System.Text;
using System.Text.Json;

namespace Plant_Explorer.Services.Services
{
    public class ScanHistoryService : IScanHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _plantIdApiKey;
        private readonly string _plantIdIdentifyUrl;
        private readonly string _plantIdRetrieveUrl;
        private readonly string _deepSeekAiApi;
        private readonly IImageService _imageService;

        public ScanHistoryService(IMemoryCache memoryCache
            , IHttpClientFactory httpClientFactory
            , IConfiguration configuration
            , IMapper mapper
            , IUnitOfWork unitOfWork
            , IImageService imageService)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _plantIdApiKey = configuration["PlantId:ApiKey"];
            _plantIdIdentifyUrl = configuration["PlantId:IdentifyUrl"];
            _plantIdRetrieveUrl = configuration["PlantId:RetrieveUrl"];
            _deepSeekAiApi = configuration["DeepSeek:ApiKey"];
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<IEnumerable<ScanHistoryGetModel>> GetAllScanHistoriesAsync()
        {
            List<ScanHistory> scanHistories = (List<ScanHistory>)await _unitOfWork.GetRepository<ScanHistory>().GetAllAsync();
            return _mapper.Map<IEnumerable<ScanHistoryGetModel>>(scanHistories);
        }

        public async Task<ScanHistoryGetModel?> GetScanHistoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid scan history ID");

            ScanHistory scanHistory = await _unitOfWork.GetRepository<ScanHistory>().GetByIdAsync(id);
            return scanHistory != null ? _mapper.Map<ScanHistoryGetModel>(scanHistory) : null;
        }

        public async Task<ScanHistoryGetModel> CreateScanHistoryAsync(ScanHistoryPostModel model)
        {
            ScanHistory scanHistoryEntity = _mapper.Map<ScanHistory>(model);
            await _unitOfWork.GetRepository<ScanHistory>().InsertAsync(scanHistoryEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ScanHistoryGetModel>(scanHistoryEntity);
        }

        public async Task<string> IdentifyPlantAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");
            // Define the folder path to save the file
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            // Ensure the folder exists
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            // Generate a unique file name
            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);
            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }


            // Convert to base64
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            string base64Image = Convert.ToBase64String(imageBytes);

            using HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _plantIdIdentifyUrl)
            {
                Headers = { { "Api-Key", _plantIdApiKey } },
                Content = new MultipartFormDataContent { { new StringContent(base64Image), "images" } }
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            PlantIdIdentifyResponse? result = JsonSerializer.Deserialize<PlantIdIdentifyResponse>(await response.Content.ReadAsStringAsync());

            if (result?.AccessToken == null)
                throw new InvalidOperationException("Failed to retrieve access token from PlantID.");

            string cacheKey = Guid.NewGuid().ToString();
            string documentId = await _imageService.UploadImageAsync(filePath);
            _memoryCache.Set(cacheKey, new CachedImage { ImageBytes = imageBytes, AccessToken = result.AccessToken, ImageDocumentId = documentId });

            // Delete the file after processing
            if (File.Exists(filePath))
                File.Delete(filePath);

            return cacheKey;
        }
        public async Task<(PlantGetModel, ScanHistoryGetModel)> GetPlantInfoAsync(string cacheKey, Guid userId)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out CachedImage cachedImage))
                throw new KeyNotFoundException("Image not found or cache expired");

            using HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _plantIdRetrieveUrl.Replace("{access_token}", cachedImage.AccessToken))
            {
                Headers = { { "Api-Key", _plantIdApiKey } }
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            PlantIdResultsResponse? plantResult = JsonSerializer.Deserialize<PlantIdResultsResponse>(await response.Content.ReadAsStringAsync());

            string scientificName = plantResult?.Result?.Classification?.Suggestions?.FirstOrDefault()?.Name!;
            if (string.IsNullOrEmpty(scientificName))
                throw new KeyNotFoundException("Scientific name not found.");
            

            //Check if plant already exists in database
            Plant? existingPlant = await _unitOfWork.GetRepository<Plant>()
                .Entities
                .Where(p => p.ScientificName!.Equals(scientificName))
                .FirstOrDefaultAsync();
            //If plant does not exist, create new plant
            if (existingPlant == null)
            {
                //Fetch plant details from GBIF
                string matchUrl = $"https://api.gbif.org/v1/species/match?name={Uri.EscapeDataString(scientificName)}";
                GbifMatchResponse? matchResult = JsonSerializer.Deserialize<GbifMatchResponse>(await httpClient.GetStringAsync(matchUrl));

                if (matchResult?.UsageKey == null)
                    throw new KeyNotFoundException("Plant not found in GBIF database.");

                string detailsUrl = $"https://api.gbif.org/v1/species/{matchResult.UsageKey}";
                GbifSpeciesResponse? detailsResult = JsonSerializer.Deserialize<GbifSpeciesResponse>(await httpClient.GetStringAsync(detailsUrl));
                

                // Fetch description from Wikipedia
                string wikiUrl = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(scientificName)}";
                WikipediaResponse? wikiResult = JsonSerializer.Deserialize<WikipediaResponse>(await httpClient.GetStringAsync(wikiUrl));
                string description = wikiResult?.Extract ?? "Description not available";
                //Fetch habitat, distribution from openAI
                /*var result = await GeneratePlantInfoAsync(scientificName);*/

                Plant newPlant = new Plant
                {
                    Name = detailsResult.VernacularName,
                    ScientificName = scientificName,
                    Family = detailsResult.Family,
                    Description = description,
                    Habitat = "not implemented yet",
                    Distribution = "not implemented yet"
                };
                await _unitOfWork.GetRepository<Plant>().InsertAsync(newPlant);
                await _unitOfWork.SaveAsync();

                existingPlant = newPlant;
            }

            ScanHistoryGetModel? scanHistory = await CreateScanHistoryAsync(new ScanHistoryPostModel
            {
                UserId = userId,
                PlantId = existingPlant.Id,
                Probability = (decimal) plantResult.Result.Classification.Suggestions.FirstOrDefault().Probability,
                ImgUrl = cachedImage.ImageDocumentId
            });

            return new (_mapper.Map<PlantGetModel>(existingPlant), scanHistory);
        }
        public async Task<byte[]> GetPlantImageAsync(string cacheKey)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out CachedImage cachedImage))
                throw new KeyNotFoundException("Image not found or cache expired");
            return cachedImage.ImageBytes;
        }

        //Let open ai to generate distribution and habitat
        private async Task<(string distribution, string habitat)> GeneratePlantInfoAsync(string plantName)
        {
            using HttpClient httpClient = _httpClientFactory.CreateClient();

            string prompt = $"Provide separate information about the distribution and habitat of {plantName}. " +
                            "First, write 'Distribution:' followed by the description. " +
                            "Then, write 'Habitat:' followed by the description.";

            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
            new { role = "system", content = "You are an expert botanist providing detailed plant information." },
            new { role = "user", content = prompt }
        },
                max_tokens = 300
            };

            string jsonBody = JsonSerializer.Serialize(requestBody);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/v1/chat/completions")
            {
                Headers = { { "Authorization", $"Bearer {_deepSeekAiApi}" } },
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Body: {responseBody}");

            response.EnsureSuccessStatusCode();
            using JsonDocument doc = JsonDocument.Parse(responseBody);
            string generatedText = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            // Extract Distribution and Habitat
            string distribution = ExtractSection(generatedText, "Distribution:");
            string habitat = ExtractSection(generatedText, "Habitat:");

            return (distribution, habitat);
        }

        // Helper method to extract section content to separate field
        private string ExtractSection(string text, string section)
        {
            int startIndex = text.IndexOf(section);
            if (startIndex == -1) return "";

            startIndex += section.Length;
            int endIndex = text.IndexOf("\n", startIndex);
            return endIndex == -1 ? text[startIndex..].Trim() : text[startIndex..endIndex].Trim();
        }
    }
}

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

        public ScanHistoryService(IMemoryCache memoryCache
            , IHttpClientFactory httpClientFactory
            , IConfiguration configuration
            , IMapper mapper
            , IUnitOfWork unitOfWork)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _plantIdApiKey = configuration["PlantId:ApiKey"];
            _plantIdIdentifyUrl = configuration["PlantId:IdentifyUrl"];
            _plantIdRetrieveUrl = configuration["PlantId:RetrieveUrl"];
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            string base64Image = Convert.ToBase64String(imageBytes);

            using var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _plantIdIdentifyUrl)
            {
                Headers = { { "Api-Key", _plantIdApiKey } },
                Content = new MultipartFormDataContent { { new StringContent(base64Image), "images" } }
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<PlantIdIdentifyResponse>(await response.Content.ReadAsStringAsync());

            if (result?.AccessToken == null)
                throw new InvalidOperationException("Failed to retrieve access token from PlantID.");

            string cacheKey = Guid.NewGuid().ToString();
            _memoryCache.Set(cacheKey, new CachedImage { ImageBytes = imageBytes, AccessToken = result.AccessToken });

            return cacheKey;
        }
        public async Task<(PlantGetModel, ScanHistoryGetModel)> GetPlantInfoAsync(string cacheKey)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out CachedImage cachedImage))
                throw new KeyNotFoundException("Image not found or cache expired");

            using var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, _plantIdRetrieveUrl.Replace("{access_token}", cachedImage.AccessToken))
            {
                Headers = { { "Api-Key", _plantIdApiKey } }
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var plantResult = JsonSerializer.Deserialize<PlantIdResultsResponse>(await response.Content.ReadAsStringAsync());

            string scientificName = plantResult?.Result?.Classification?.Suggestions?.FirstOrDefault()?.Name;
            if (string.IsNullOrEmpty(scientificName))
                throw new KeyNotFoundException("Scientific name not found.");

            string matchUrl = $"https://api.gbif.org/v1/species/match?name={Uri.EscapeDataString(scientificName)}";
            var matchResult = JsonSerializer.Deserialize<GbifMatchResponse>(await httpClient.GetStringAsync(matchUrl));

            if (matchResult?.UsageKey == null)
                throw new KeyNotFoundException("Plant not found in GBIF database.");

            string detailsUrl = $"https://api.gbif.org/v1/species/{matchResult.UsageKey}";
            var detailsResult = JsonSerializer.Deserialize<GbifSpeciesResponse>(await httpClient.GetStringAsync(detailsUrl));

            //Check if plant already exists in database
            Plant existingPlant = await _unitOfWork.GetRepository<Plant>()
                .Entities
                .Where(p => p.ScientificName!.Equals(scientificName))
                .FirstOrDefaultAsync();
            //If plant does not exist, create new plant
            if (existingPlant == null)
            {
                Plant newPlant = new Plant
                {
                    Name = detailsResult.VernacularName,
                    ScientificName = scientificName,
                    Family = detailsResult.Family,
                    Description = "Description not implemented yet",
                    Habitat = "Habitat not implemented yet",
                    Distribution = "Distribution not implemented yet"
                };
                await _unitOfWork.GetRepository<Plant>().InsertAsync(newPlant);
                await _unitOfWork.SaveAsync();

                existingPlant = newPlant;
            }

            var scanHistory = await CreateScanHistoryAsync(new ScanHistoryPostModel
            {
                UserId = Guid.Parse("0E2BE45F-B2C9-4F2F-9879-4AAAAA17BE65"),
                PlantId = existingPlant.Id,
                Probability = (decimal)plantResult.Result.Classification.Suggestions.FirstOrDefault().Probability,
                ImgUrl = "Image not implemented yet"
            });

            return new (_mapper.Map<PlantGetModel>(existingPlant), scanHistory);
        }
        public async Task<byte[]> GetPlantImageAsync(string cacheKey)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out CachedImage cachedImage))
                throw new KeyNotFoundException("Image not found or cache expired");
            return cachedImage.ImageBytes;
        }
    }
}

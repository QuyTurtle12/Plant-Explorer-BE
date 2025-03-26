using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Controller for handling scan history related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ScanHistoryController : ControllerBase
    {
        private readonly IScanHistoryService _scanHistoryService;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanHistoryController"/> class.
        /// </summary>
        /// <param name="scanHistoryService">The scan history service.</param>
        public ScanHistoryController(IScanHistoryService scanHistoryService, ITokenService tokenService)
        {
            _scanHistoryService = scanHistoryService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Identifies the plant from the uploaded image file.
        /// </summary>
        /// <param name="file">The image file containing the plant.</param>
        /// <returns>The identification result.</returns>
        [HttpPost]
        [Route("/api/scan-histories/identify")]
        public async Task<IActionResult> IdentifyPlant(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var result = await _scanHistoryService.IdentifyPlantAsync(file);
                if (result == null)
                    return BadRequest("Failed to identify plant");

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to identify plant");
            }
        }

        /// <summary>
        /// Gets the plant information based on the cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>The plant information and scan history.</returns>
        [HttpGet]
        [Route("/api/scan-histories/plant-info/{cacheKey}")]
        public async Task<IActionResult> GetPlantInfo(string cacheKey)
        {
            string? userId = _tokenService.GetCurrentUserId();
            if (userId == null) return StatusCode(401, "Not authorized");

            try
            {
                var (plant, scanHistory) = await _scanHistoryService.GetPlantInfoAsync(cacheKey, Guid.Parse(userId));
                if (plant == null || scanHistory == null)
                    return NotFound("Plant details not found.");

                return Ok(new { Plant = plant, ScanHistory = scanHistory });
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to retrieve plant details.");
            }
        }

        /// <summary>
        /// Gets the plant image based on the cache key.
        /// </summary>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>The plant image.</returns>
        [HttpGet]
        [Route("/api/scan-histories/image/{cacheKey}")]
        public async Task<IActionResult> GetPlantImage(string cacheKey)
        {
            try
            {
                var imageResult = await _scanHistoryService.GetPlantImageAsync(cacheKey);
                if (imageResult == null)
                    return NotFound("Image not found or cache expired");

                return File(imageResult, "image/jpeg");
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to retrieve plant image");
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        /// <summary>
        /// Endpoint to upload an image.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/images")]
        public async Task<IActionResult> Upload([FromQuery] string imagePath)
        {
            try
            {
                // Capture the documentId returned by the service.
                string documentId = await _imageService.UploadImageAsync(imagePath);
                return Ok(new { message = "Image uploaded successfully.", documentId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint to retrieve an base-64 image by its document ID.
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/images/{documentId}")]
        public async Task<IActionResult> Get(string documentId)
        {
            try
            {
                var imageRecord = await _imageService.GetImageAsync(documentId);
                return Ok(imageRecord);
            }
            catch (System.Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }

        }

        /// <summary>
        /// Display image
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/images/display/{documentId}")]
        public async Task<IActionResult> Display(string documentId)
        {
            try
            {
                var imageRecord = await _imageService.GetImageAsync(documentId);
                byte[] imageBytes = Convert.FromBase64String(imageRecord.ImageData);
                return File(imageBytes, "image/png");
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
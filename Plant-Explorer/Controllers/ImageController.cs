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

        // Endpoint to upload an image.
        // For simplicity, we assume a query parameter or a file path is provided.
        // In a real-world app, you'd use IFormFile from a multipart/form-data request.
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromQuery] string imagePath)
        {
            try
            {
                await _imageService.UploadImageAsync(imagePath);
                return Ok(new { message = "Image uploaded successfully." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint to retrieve an image by its document ID.
        [HttpGet("get/{documentId}")]
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
        [HttpGet("display/{documentId}")]
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
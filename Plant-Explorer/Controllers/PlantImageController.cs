using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews.PlantImageModel;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantImageController : ControllerBase
    {
        private readonly IPlantImageService _plantImageService;

        public PlantImageController(IPlantImageService plantImageService)
        {
            _plantImageService = plantImageService;
        }

        [HttpGet]
        [Route("/api/plant-images")]
        public async Task<IActionResult> GetAll()
        {
            var images = await _plantImageService.GetAllPlantImagesAsync();
            return Ok(images);
        }

        [HttpGet]
        [Route("/api/plant-images/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var image = await _plantImageService.GetPlantImageByIdAsync(id);
            return Ok(image);
        }

        [HttpPost]
        [Route("/api/plant-images")]
        public async Task<IActionResult> Create([FromBody] CreatePlantImageRequest request)
        {
            var image = await _plantImageService.CreatePlantImageAsync(request);
            return Ok(image);
        }

        [HttpPut]
        [Route("/api/plant-images")]
        public async Task<IActionResult> Update([FromBody] UpdatePlantImageRequest request)
        {
            var image = await _plantImageService.UpdatePlantImageAsync(request);
            return Ok(image);
        }

        [HttpDelete]
        [Route("/api/plant-images/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _plantImageService.DeletePlantImageAsync(id);
            return Ok(new { message = "Plant image deleted successfully" });
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Controller for managing plants.
    /// </summary>
    [Route("api/plants")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantController"/> class.
        /// </summary>
        /// <param name="plantService">The plant service.</param>
        public PlantController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        /// <summary>
        /// Gets all plants.
        /// </summary>
        /// <returns>A list of plants.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPlants()
        {
            var plants = await _plantService.GetAllPlantsAsync();
            return Ok(plants);
        }

        /// <summary>
        /// Gets a plant by its identifier.
        /// </summary>
        /// <param name="id">The plant identifier.</param>
        /// <returns>The plant with the specified identifier.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlantById(Guid id)
        {
            var plant = await _plantService.GetPlantByIdAsync(id);
            if (plant == null)
                return NotFound();
            return Ok(plant);
        }

        /// <summary>
        /// Creates a new plant.
        /// </summary>
        /// <param name="model">The plant model.</param>
        /// <returns>The created plant.</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePlant(PlantPostModel model)
        {
            var plant = await _plantService.CreatePlantAsync(model);
            return CreatedAtAction(nameof(GetPlantById), new { id = plant.Id }, plant);
        }

        /// <summary>
        /// Updates an existing plant.
        /// </summary>
        /// <param name="id">The plant identifier.</param>
        /// <param name="model">The plant model.</param>
        /// <returns>The updated plant.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlant(Guid id, PlantPutModel model)
        {
            var updatedPlant = await _plantService.UpdatePlantAsync(id, model);
            
            if (updatedPlant == null)
                return NotFound();
            return Ok(updatedPlant);
        }

        /// <summary>
        /// Deletes a plant by its identifier.
        /// </summary>
        /// <param name="id">The plant identifier.</param>
        /// <returns>No content if the plant was deleted successfully.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(Guid id)
        {
            var result = await _plantService.SoftDeletePlantAsync(id);
            if (!result)
                return BadRequest("Delete fail");
            return Ok("Delete success");
        }
        /// <summary>
        /// Search plants by name/scientific name.
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        [HttpGet("/searchByName")]
        public async Task<IActionResult> SearchPlantByName(string? searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return Ok(await _plantService.GetAllPlantsAsync());
            }
            var plants = await _plantService.SearchPlantsByName(searchString);
            if (plants == null || plants.Count() == 0)
                return NotFound();

            return Ok(plants);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Controller for managing plant applications.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlantApplicationController : ControllerBase
    {
        private readonly IPlantApplicationService _plantApplicationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantApplicationController"/> class.
        /// </summary>
        /// <param name="plantApplicationService">The plant application service.</param>
        public PlantApplicationController(IPlantApplicationService plantApplicationService)
        {
            _plantApplicationService = plantApplicationService;
        }

        /// <summary>
        /// Gets all plant applications.
        /// </summary>
        /// <returns>A list of plant applications.</returns>
        [HttpGet]
        [Route("/api/plant-applications")]
        public async Task<ActionResult<IEnumerable<PlantApplicationGetModel>>> GetAll()
        {
            return Ok(await _plantApplicationService.GetAllAsync());
        }

        /// <summary>
        /// Gets a plant application by ID.
        /// </summary>
        /// <param name="id">The ID of the plant application.</param>
        /// <returns>The plant application with the specified ID.</returns>
        [HttpGet]
        [Route("/api/plant-applications/{id}")]
        public async Task<ActionResult<PlantApplicationGetModel>> GetById(Guid id)
        {
            var result = await _plantApplicationService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new plant application.
        /// </summary>
        /// <param name="model">The plant application model.</param>
        /// <returns>The created plant application.</returns>
        [HttpPost]
        [Route("/api/plant-applications")]
        public async Task<ActionResult<PlantApplicationGetModel>> Create(PlantApplicationPostModel model)
        {
            var result = await _plantApplicationService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing plant application.
        /// </summary>
        /// <param name="id">The ID of the plant application to update.</param>
        /// <param name="model">The updated plant application model.</param>
        /// <returns>The updated plant application.</returns>
        [HttpPut]
        [Route("/api/plant-applications/{id}")]
        public async Task<ActionResult<PlantApplicationGetModel>> Update(Guid id, PlantApplicationPutModel model)
        {
            var result = await _plantApplicationService.UpdateAsync(id, model);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Deletes a plant application by ID.
        /// </summary>
        /// <param name="id">The ID of the plant application to delete.</param>
        /// <returns>No content if the deletion was successful.</returns>
        [HttpDelete]
        [Route("/api/plant-applications/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _plantApplicationService.DeleteAsync(id);
            if (!success) return NotFound();
            return Ok("Delete success");
        }
    }
}

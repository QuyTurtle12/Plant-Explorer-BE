using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Controller for managing plant characteristics.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlantCharacteristicController : ControllerBase
    {
        private readonly IPlantCharacteristicService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantCharacteristicController"/> class.
        /// </summary>
        /// <param name="service">The plant characteristic service.</param>
        public PlantCharacteristicController(IPlantCharacteristicService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all plant characteristics.
        /// </summary>
        /// <returns>A list of plant characteristics.</returns>
        [HttpGet]
        [Route("/api/plant-characteristics")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllCharacteristicsAsync();
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Gets a plant characteristic by ID.
        /// </summary>
        /// <param name="id">The ID of the plant characteristic.</param>
        /// <returns>The plant characteristic with the specified ID.</returns>
        [HttpGet]
        [Route("/api/plant-characteristics/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetCharacteristicByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Creates a new plant characteristic.
        /// </summary>
        /// <param name="model">The plant characteristic model.</param>
        /// <returns>The created plant characteristic.</returns>
        [HttpPost]
        [Route("/api/plant-characteristics")]
        public async Task<IActionResult> Create([FromBody] PlantCharacteristicPostModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateCharacteristicAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing plant characteristic.
        /// </summary>
        /// <param name="id">The ID of the plant characteristic to update.</param>
        /// <param name="model">The updated plant characteristic model.</param>
        /// <returns>The updated plant characteristic.</returns>
        [HttpPut]
        [Route("/api/plant-characteristics/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PlantCharacteristicPutModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateCharacteristicAsync(id, model);
            return updated != null ? Ok(updated) : NotFound();
        }

        /// <summary>
        /// Deletes a plant characteristic by ID.
        /// </summary>
        /// <param name="id">The ID of the plant characteristic to delete.</param>
        /// <returns>No content if the deletion was successful, otherwise not found.</returns>
        [HttpDelete]
        [Route("/api/plant-characteristics/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _service.DeleteCharacteristicAsync(id);
            return success ? Ok("Delete success") : NotFound();
        }
    }
}

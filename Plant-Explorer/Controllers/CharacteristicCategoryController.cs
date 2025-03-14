using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Controller for managing characteristic categories.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CharacteristicCategoryController : ControllerBase
    {
        private readonly ICharacteristicCategoryService _characteristicCategoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacteristicCategoryController"/> class.
        /// </summary>
        /// <param name="service">The characteristic category service.</param>
        public CharacteristicCategoryController(ICharacteristicCategoryService service)
        {
            _characteristicCategoryService = service;
        }

        /// <summary>
        /// Gets all characteristic categories.
        /// </summary>
        /// <returns>A list of characteristic categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _characteristicCategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Gets a characteristic category by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the characteristic category.</param>
        /// <returns>The characteristic category.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _characteristicCategoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>
        /// Creates a new characteristic category.
        /// </summary>
        /// <param name="model">The characteristic category model.</param>
        /// <returns>The created characteristic category.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CharacteristicCategoryPostModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var createdCategory = await _characteristicCategoryService.CreateCategoryAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Updates an existing characteristic category.
        /// </summary>
        /// <param name="id">The identifier of the characteristic category.</param>
        /// <param name="model">The characteristic category model.</param>
        /// <returns>The updated characteristic category.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CharacteristicCategoryPutModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updatedCategory = await _characteristicCategoryService.UpdateCategoryAsync(id, model);
            if (updatedCategory == null) return NotFound();
            return Ok(updatedCategory);
        }

        /// <summary>
        /// Deletes a characteristic category by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the characteristic category.</param>
        /// <returns>No content if the deletion was successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _characteristicCategoryService.SoftDeleteCharacteristicCategoryAsync(id);
            if (!deleted) return BadRequest("Delete failed");
            return Ok("Delete success");
        }
    }
}

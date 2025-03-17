using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Services.Interface;
using static Plant_Explorer.Contract.Repositories.ModelViews.ApplicationCategoryModels;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// Controller for managing application categories.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationCategoryController : ControllerBase
    {
        private readonly IApplicationCategoryService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCategoryController"/> class.
        /// </summary>
        /// <param name="service">The application category service.</param>
        public ApplicationCategoryController(IApplicationCategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all application categories.
        /// </summary>
        /// <returns>A list of application categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllCategoriesAsync());
        }

        /// <summary>
        /// Gets an application category by ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>The application category.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            return category != null ? Ok(category) : NotFound();
        }

        /// <summary>
        /// Creates a new application category.
        /// </summary>
        /// <param name="model">The category model.</param>
        /// <returns>The created category.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationCategoryPostModel model)
        {
            var createdCategory = await _service.CreateCategoryAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        /// <summary>
        /// Updates an existing application category.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <param name="model">The updated category model.</param>
        /// <returns>The updated category.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ApplicationCategoryPutModel model)
        {
            var updatedCategory = await _service.UpdateCategoryAsync(id, model);
            return updatedCategory != null ? Ok(updatedCategory) : NotFound();
        }

        /// <summary>
        /// Deletes an application category by ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>No content if successful, otherwise not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _service.SoftDeleteApplicationCategoryAsync(id) ? Ok("Delete success") : BadRequest("Delete failed");
        }
    }
}

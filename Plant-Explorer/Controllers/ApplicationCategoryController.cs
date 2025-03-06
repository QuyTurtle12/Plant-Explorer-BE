using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Services.Interface;
using static Plant_Explorer.Contract.Repositories.ModelViews.ApplicationCategoryModels;

namespace Plant_Explorer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationCategoryController : ControllerBase
    {
        private readonly IApplicationCategoryService _service;

        public ApplicationCategoryController(IApplicationCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllCategoriesAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            return category != null ? Ok(category) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationCategoryPostModel model)
        {
            var createdCategory = await _service.CreateCategoryAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ApplicationCategoryPutModel model)
        {
            var updatedCategory = await _service.UpdateCategoryAsync(id, model);
            return updatedCategory != null ? Ok(updatedCategory) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _service.SoftDeleteApplicationCategoryAsync(id) ? NoContent() : NotFound();
        }
    }
}

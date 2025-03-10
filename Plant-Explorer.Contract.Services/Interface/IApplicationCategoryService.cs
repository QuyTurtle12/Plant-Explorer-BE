using static Plant_Explorer.Contract.Repositories.ModelViews.ApplicationCategoryModels;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IApplicationCategoryService
    {
        Task<IEnumerable<ApplicationCategoryGetModel>> GetAllCategoriesAsync();
        Task<ApplicationCategoryGetModel?> GetCategoryByIdAsync(Guid id);
        Task<ApplicationCategoryGetModel> CreateCategoryAsync(ApplicationCategoryPostModel model);
        Task<ApplicationCategoryGetModel?> UpdateCategoryAsync(Guid id, ApplicationCategoryPutModel model);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<bool> SoftDeleteApplicationCategoryAsync(Guid id);
    }
}

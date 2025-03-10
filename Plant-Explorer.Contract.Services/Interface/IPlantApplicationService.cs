using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IPlantApplicationService
    {
        Task<IEnumerable<PlantApplicationGetModel>> GetAllAsync();
        Task<PlantApplicationGetModel?> GetByIdAsync(Guid id);
        Task<PlantApplicationGetModel> CreateAsync(PlantApplicationPostModel model);
        Task<PlantApplicationGetModel?> UpdateAsync(Guid id, PlantApplicationPutModel model);
        Task<bool> DeleteAsync(Guid id);
    }
}

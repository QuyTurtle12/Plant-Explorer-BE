using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IPlantService
    {
        Task<IEnumerable<PlantGetModel>> GetAllPlantsAsync();
        Task<PlantGetModel?> GetPlantByIdAsync(Guid id);
        Task<PlantGetModel> CreatePlantAsync(PlantPostModel model);
        Task<PlantGetModel?> UpdatePlantAsync(Guid id, PlantPutModel model);
        Task<bool> DeletePlantAsync(Guid id);
        Task<PlantGetModel?> GetPlantByScientificName(string scientificName);
        Task<IEnumerable<PlantGetModel>> SearchPlantsByName(string searchStringName);
    }
}

using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface ICharacteristicCategoryService
    {
        Task<IEnumerable<CharacteristicCategoryGetModel>> GetAllCategoriesAsync();
        Task<CharacteristicCategoryGetModel?> GetCategoryByIdAsync(Guid id);
        Task<CharacteristicCategoryGetModel> CreateCategoryAsync(CharacteristicCategoryPostModel model);
        Task<CharacteristicCategoryGetModel?> UpdateCategoryAsync(Guid id, CharacteristicCategoryPutModel model);
        Task<bool> DeleteCategoryAsync(Guid id);
        Task<bool> SoftDeleteCharacteristicCategoryAsync(Guid id);
    }
}

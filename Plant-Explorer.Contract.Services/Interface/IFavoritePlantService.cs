using Plant_Explorer.Contract.Repositories.ModelViews.FavoritePlantModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IFavoritePlantService
    {
        Task<PaginatedList<GetFavoritePlantModel>> GetUserFavoritePlantsAsync(int index, int pageSize);
        Task CreateUserFavoritePlantAsync(PostFavoritePlantModel newFavoritePlant);
        Task DeleteUserFavoritePlantAsync(string id);
    }
}

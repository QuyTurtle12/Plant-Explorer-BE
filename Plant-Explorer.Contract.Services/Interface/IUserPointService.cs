using Plant_Explorer.Contract.Repositories.ModelViews.UserPointModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IUserPointService
    {
        Task<PaginatedList<GetUserPointModel>> GetAllUserPointsAsync(int index, int pageSize, string? idSearch, string? userIdSearch);
        Task<GetUserPointModel> GetCurrentUserPointdAsync();
        Task CreateUserPointAsync(PostUserPointModel newUserPoint);
        Task UpdateUserPointAsync(PutUserPointModel updatedUserPoint);
    }
}

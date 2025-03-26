using Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IUserBadgeService
    {
        Task<PaginatedList<GetUserBadgeModel>> GetUserBadgesAsync(int index, int pageSize);
        Task CreateUserBadgeAsync(PostUserBadgeModel newUserBadge);
    }
}

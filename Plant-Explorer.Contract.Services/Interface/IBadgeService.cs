using Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Core.Constants.Enum.EnumBadge;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IBadgeService
    {
        Task<PaginatedList<GetBadgeModel>> GetAllBadgesAsync(int index, int pageSize, string? idSearch, string? nameSearch, EnumBadge? badgeType);
        Task<GetBadgeModel> GetBadgeByIdAsync(string id);
        Task CreateBadgeAsync(PostBadgeModel newBadge);
        Task UpdateBadgeAsync(string id, PutBadgeModel updatedBadge);
        Task DeleteBadgeAsync(string id);
    }
}

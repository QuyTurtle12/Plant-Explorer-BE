using Plant_Explorer.Contract.Repositories.ModelViews.BugReportModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IBugReportService
    {
        Task<PaginatedList<GetBugReportModel>> GetAllUserReportsAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? userIdSearch);
        Task CreateUserReportAsync(PostBugReportModel newUserBadge);
    }
}

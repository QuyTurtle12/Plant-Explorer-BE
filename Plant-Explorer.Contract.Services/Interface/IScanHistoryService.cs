using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IScanHistoryService
    {
        Task<IEnumerable<ScanHistoryGetModel>> GetAllScanHistoriesAsync();
        Task<ScanHistoryGetModel?> GetScanHistoryByIdAsync(Guid id);
        Task<ScanHistoryGetModel> CreateScanHistoryAsync(ScanHistoryPostModel model);
    }
}

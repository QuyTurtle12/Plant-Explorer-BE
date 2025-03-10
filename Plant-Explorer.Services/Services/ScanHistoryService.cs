using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
namespace Plant_Explorer.Services.Services
{
    public class ScanHistoryService : IScanHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ScanHistoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ScanHistoryGetModel>> GetAllScanHistoriesAsync()
        {
            List<ScanHistory> scanHistories = (List<ScanHistory>)await _unitOfWork.GetRepository<ScanHistory>().GetAllAsync();
            return _mapper.Map<IEnumerable<ScanHistoryGetModel>>(scanHistories);
        }

        public async Task<ScanHistoryGetModel?> GetScanHistoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid scan history ID");

            ScanHistory scanHistory = await _unitOfWork.GetRepository<ScanHistory>().GetByIdAsync(id);
            return scanHistory != null ? _mapper.Map<ScanHistoryGetModel>(scanHistory) : null;
        }

        public async Task<ScanHistoryGetModel> CreateScanHistoryAsync(ScanHistoryPostModel model)
        {
            ScanHistory scanHistoryEntity = _mapper.Map<ScanHistory>(model);
            await _unitOfWork.GetRepository<ScanHistory>().InsertAsync(scanHistoryEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ScanHistoryGetModel>(scanHistoryEntity);
        }
    }
}

using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Services.MapperProfile
{
    public class ScanHistoryProfile : Profile
    {
        public ScanHistoryProfile()
        {
            CreateMap<ScanHistory, ScanHistoryBaseModel>().ReverseMap();
            CreateMap<ScanHistory, ScanHistoryGetModel > ().ReverseMap();
            CreateMap<ScanHistory, ScanHistoryPostModel>().ReverseMap();
        }
    }
}

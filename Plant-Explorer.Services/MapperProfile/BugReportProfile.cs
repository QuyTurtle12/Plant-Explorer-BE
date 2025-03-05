using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.BugReportModel;

namespace Plant_Explorer.Services.MapperProfile
{
    public class BugReportProfile : Profile
    {
        public BugReportProfile() 
        {
            CreateMap<GetBugReportModel, BugReport>().ReverseMap();
            CreateMap<PostBugReportModel, BugReport>().ReverseMap();
        }
    }
}

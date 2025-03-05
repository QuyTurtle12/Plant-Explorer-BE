using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using static Plant_Explorer.Contract.Repositories.ModelViews.ApplicationCategoryModels;

namespace Plant_Explorer.Services.MapperProfile
{
    public class ApplicationCategoryProfile : Profile
    {
        public ApplicationCategoryProfile()
        {
            CreateMap<ApplicationCategory, ApplicationCategoryGetModel>().ReverseMap();
            CreateMap<ApplicationCategory, ApplicationCategoryPostModel>().ReverseMap();
            CreateMap<ApplicationCategory, ApplicationCategoryPutModel>().ReverseMap();
        }
    }
}

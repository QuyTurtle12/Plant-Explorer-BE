using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Services.MapperProfile
{
    public class PlantApplicationProfile : Profile
    {
        public PlantApplicationProfile()
        {
            CreateMap<PlantApplication, PlantApplicationGetModel>().ReverseMap();
            CreateMap<PlantApplication, PlantApplicationPostModel>().ReverseMap();
            CreateMap<PlantApplication, PlantApplicationPutModel>().ReverseMap();
        }
    }
}

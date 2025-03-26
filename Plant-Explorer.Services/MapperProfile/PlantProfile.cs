using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Services.MapperProfile
{
    public class PlantProfile : Profile
    {
        public PlantProfile()
        {
            CreateMap<Plant, PlantGetModel>().ReverseMap();
            CreateMap<Plant, PlantPostModel>().ReverseMap();
            CreateMap<Plant, PlantPutModel>().ReverseMap();
        }
    }
}

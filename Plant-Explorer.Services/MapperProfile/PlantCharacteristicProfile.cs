using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Services.MapperProfile
{
    public class PlantCharacteristicProfile : Profile
    {
        public PlantCharacteristicProfile()
        {
            CreateMap<PlantCharacteristic, PlantCharacteristicGetModel>().ReverseMap();
            CreateMap<PlantCharacteristic, PlantCharacteristicPostModel>().ReverseMap();
            CreateMap<PlantCharacteristic, PlantCharacteristicPutModel>().ReverseMap();
        }
    }
}

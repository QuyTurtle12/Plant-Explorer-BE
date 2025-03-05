using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews;

namespace Plant_Explorer.Services.MapperProfile
{
    public class CharacteristicCategoryProfile : Profile
    {
        public CharacteristicCategoryProfile()
        {
            CreateMap<CharacteristicCategory, CharacteristicCategoryGetModel>().ReverseMap();
            CreateMap<CharacteristicCategory, CharacteristicCategoryPostModel>().ReverseMap();
            CreateMap<CharacteristicCategory, CharacteristicCategoryPutModel>().ReverseMap();
        }
    }
}

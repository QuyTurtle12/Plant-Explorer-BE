using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.FavoritePlantModel;

namespace Plant_Explorer.Services.MapperProfile
{
    public class FavoritePlantProfile : Profile
    {
        public FavoritePlantProfile() 
        {
            CreateMap<GetFavoritePlantModel, FavoritePlant>().ReverseMap();
            CreateMap<PostFavoritePlantModel, FavoritePlant>().ReverseMap();
        }
    }
}

using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.UserPointModel;

namespace Plant_Explorer.Services.MapperProfile
{
    public class UserPointProfile : Profile
    {
        public UserPointProfile() 
        { 
            CreateMap<GetUserPointModel, UserPoint>().ReverseMap();
            CreateMap<PostUserPointModel, UserPoint>().ReverseMap();
            CreateMap<PutUserPointModel, UserPoint>().ReverseMap();
        }
    }
}

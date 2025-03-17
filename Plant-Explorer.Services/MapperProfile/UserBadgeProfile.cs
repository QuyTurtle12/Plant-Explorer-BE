using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel;

namespace Plant_Explorer.Services.MapperProfile
{
    public class UserBadgeProfile : Profile
    {
        public UserBadgeProfile() 
        {
            CreateMap<GetUserBadgeModel, UserBadge>().ReverseMap();
            CreateMap<PostUserBadgeModel, UserBadge>().ReverseMap();
        }
    }
}

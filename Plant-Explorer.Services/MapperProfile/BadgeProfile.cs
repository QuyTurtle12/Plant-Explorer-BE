using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel;

namespace Plant_Explorer.Services.MapperProfile
{
    public class BadgeProfile : Profile
    {
        public BadgeProfile() 
        {
            CreateMap<GetBadgeModel, Badge>().ReverseMap();
            CreateMap<PostBadgeModel, Badge>().ReverseMap();
            CreateMap<PutBadgeModel, Badge>().ReverseMap();
        }
    }
}

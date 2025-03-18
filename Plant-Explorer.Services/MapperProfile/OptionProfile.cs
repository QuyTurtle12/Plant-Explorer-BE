using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;

namespace Plant_Explorer.Services.MapperProfile
{
    public class OptionProfile : Profile
    {
        public OptionProfile()
        {
            CreateMap<GetOptionModel, Option>().ReverseMap();
            CreateMap<PostOptionModel, Option>().ReverseMap();
            CreateMap<PutOptionModel, Option>().ReverseMap();
        }
    }
}

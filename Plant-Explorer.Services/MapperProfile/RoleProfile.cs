using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.RoleModel;
using AutoMapper;

namespace Plant_Explorer.Services.MapperProfile
{
    public class RoleProfile : Profile
    {
        public RoleProfile() 
        {
            CreateMap<ApplicationRole, GetRoleModel>().ReverseMap();
        }
    }
}

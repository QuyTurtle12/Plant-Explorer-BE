using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Services.MapperProfile
{
    public class OptionProfile : Profile
    {
        
        
            public OptionProfile()
            {
                CreateMap<GetOptionModel, Option>();
                CreateMap<Option, GetOptionModel>();
                CreateMap<PostOptionModel, Option>().ReverseMap();
                CreateMap<PutOptionModel, Option>().ReverseMap();
            }
        
    }
    }

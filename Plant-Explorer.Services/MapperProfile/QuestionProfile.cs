using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Services.MapperProfile
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<GetQuestionModel, Question>().ReverseMap();
            CreateMap<PostQuestionModel, Question>().ReverseMap();
            CreateMap<PutQuestionModel, Question>().ReverseMap();
        }
    }
}

using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizModel;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;

namespace Plant_Explorer.Services.MapperProfile
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<GetQuizModel, Quiz>().ReverseMap();
            CreateMap<PostQuizModel, Quiz>().ReverseMap();
            CreateMap<PutQuizModel, Quiz>().ReverseMap();
        }
    }

}
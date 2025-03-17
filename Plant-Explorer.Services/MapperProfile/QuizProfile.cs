using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizModel;
using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;

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
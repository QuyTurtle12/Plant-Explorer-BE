using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Repositories.Repositories;

namespace Plant_Explorer.Services.MapperProfile;
public class QuizAttemptProfile : Profile
{
    public QuizAttemptProfile()
    {
        CreateMap<GetQuizAttemptModel, QuizAttempt>().ReverseMap();
        CreateMap<PostQuizAttemptModel, QuizAttempt>().ReverseMap();
    }
}

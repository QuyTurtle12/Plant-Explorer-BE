using Plant_Explorer.Contract.Repositories.ModelViews.QuizModel;
using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IQuizService
    {
        Task<PaginatedList<GetQuizModel>> GetAllQuizzesAsync(int index, int pageSize, string? idSearch, string? nameSearch);
        Task<GetQuizModel> GetQuizByIdAsync(string id);
        Task CreateQuizAsync(PostQuizModel newQuiz);
        Task UpdateQuizAsync(string id, PutQuizModel updatedQuiz);
        Task DeleteQuizAsync(string id);
    }
}
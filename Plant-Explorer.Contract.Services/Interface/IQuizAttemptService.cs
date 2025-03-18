using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;

namespace Plant_Explorer.Services.Services
{
    public interface IQuizAttemptService
    {
        Task<PaginatedList<GetQuizAttemptModel>> GetAllQuizAttemptsAsync(int index, int pageSize, string? userId, string? quizId);
        Task<GetQuizAttemptModel> GetQuizAttemptByIdAsync(string id);
        Task CreateQuizAttemptAsync(PostQuizAttemptModel newAttempt);
    }
}

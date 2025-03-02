using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.Quiz;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Services.Services
{
    public interface IQuizService
    {
        Task CreateQuizAsync(CreateQuizDto newQuiz);
        Task DeleteQuizAsync(string id);
        Task<PaginatedList<Quiz>> GetAllQuizzesAsync(int index, int pageSize, string? nameSearch);
        Task<Quiz> GetQuizByIdAsync(string id);
        Task UpdateQuizAsync(string id, UpdateQuizDto updatedQuiz);
    }
}

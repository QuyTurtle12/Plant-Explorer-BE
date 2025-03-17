using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IQuestionService
    {
        Task<PaginatedList<GetQuestionModel>> GetAllQuestionsAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? quizId);
        Task<GetQuestionModel> GetQuestionByIdAsync(string id);
        Task CreateQuestionAsync(PostQuestionModel newQuestion);
        Task UpdateQuestionAsync(string id, PutQuestionModel updatedQuestion);
        Task DeleteQuestionAsync(string id);
    }
}

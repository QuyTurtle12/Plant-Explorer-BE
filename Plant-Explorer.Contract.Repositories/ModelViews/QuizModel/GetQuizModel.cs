using Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel;
using Plant_Explorer.Contract.Repositories.ModelViews.QuizModel;
using System.ComponentModel.DataAnnotations;

namespace Plant_Explorer.Contract.Repositories.ModelViews.Quiz
{
    public class GetQuizModel : BaseQuizModel
    {
        public string Id { get; set; } = string.Empty;
        public string CreatedTime { get; set; } = string.Empty;
        public string LastUpdatedTime { get; set; } = string.Empty;
    }
}


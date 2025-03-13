using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel
{
    public class BaseQuestionModel
    {
        public string Name { get; set; } = string.Empty;
        public string? QuizId { get; set; }
        public string? Context { get; set; }
        public int? Point { get; set; }
        public string? ImageUrl { get; set; }
    }
}

using System;
using Plant_Explorer.Core.Constants.Enum;

namespace Plant_Explorer.Contract.Repositories.ModelViews.QuizModel
{
    public class BaseQuizModel
    {
        public string Name { get; set; } = string.Empty;
        //public string? Context { get; set; }
        //public int? Point { get; set; }
        public string? ImageUrl { get; set; }
    }
}

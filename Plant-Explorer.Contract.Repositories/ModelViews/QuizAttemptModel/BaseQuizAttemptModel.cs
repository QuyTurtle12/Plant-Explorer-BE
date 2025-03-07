using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel
{
    public class BaseQuizAttemptModel
    {
        public string? QuizId { get; set; }
        public string? UserId { get; set; }
        public DateTime AttemptTime { get; set; }
    }
}

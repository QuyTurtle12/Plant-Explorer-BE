using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel
{
    public class GetQuizAttemptModel : BaseQuizAttemptModel
    {
        public Guid Id { get; set; } 
        public string CreatedTime { get; set; } = string.Empty;
        public string LastUpdatedTime { get; set; } = string.Empty;
    }
}

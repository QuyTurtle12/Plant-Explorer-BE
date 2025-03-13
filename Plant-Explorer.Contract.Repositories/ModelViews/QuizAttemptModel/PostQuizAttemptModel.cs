using Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel;
using System;

namespace Plant_Explorer.Contract.Repositories.ModelViews.QuizAttempt
{
    public class PostQuizAttemptModel : BaseQuizAttemptModel
    {
        public int Score { get; set; }
    }
}

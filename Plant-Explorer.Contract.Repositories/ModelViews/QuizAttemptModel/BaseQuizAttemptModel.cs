namespace Plant_Explorer.Contract.Repositories.ModelViews.QuizAttemptModel
{
    public class BaseQuizAttemptModel
    {
        public Guid QuizId { get; set; }
        public DateTime AttemptTime { get; set; }
    }
}

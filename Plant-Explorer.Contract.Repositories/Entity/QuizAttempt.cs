namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class QuizAttempt
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime AttemptTime { get; set; }
        public int TotalPoint;

        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Quiz? Quiz { get; set; }

        public QuizAttempt() 
        {
            Id = Guid.NewGuid();    
        }
    }
}

using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Quiz : BaseEntity
    {
        public string? imageUrl { get; set; }
        // Navigation Properties
        public virtual ICollection<QuizAttempt>? QuizAttempts { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
    }
}

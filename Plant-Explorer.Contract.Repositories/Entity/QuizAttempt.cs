using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class QuizAttempt : BaseEntity
    {
        public Guid? QuizId { get; set; }
        public Guid? UserId { get; set; }

       
        public DateTime AttemptTime { get; set; }

        // Navigation Properties
        public virtual Quiz? Quiz { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    
}
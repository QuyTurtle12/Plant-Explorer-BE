using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Quiz : BaseEntity
    {
        public string? imageUrl { get; set; }
        // Navigation Properties
        public virtual ICollection<QuizAttempt>? QuizAttempts { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier
        public string Name { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public string? ImageUrl { get; set; }
        public int Status { get; set; } // E.g., 0 = Inactive, 1 = Active
    }
}

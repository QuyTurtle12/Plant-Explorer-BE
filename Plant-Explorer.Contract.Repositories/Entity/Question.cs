using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Question : BaseEntity
    {
        public Guid QuizId { get; set; }
        public string? Context { get; set; }
        public int? Point { get; set; }
        public string? imageUrl { get; set; }

        // Navigation Properties
        public virtual Quiz? Quiz { get; set; }
        public virtual ICollection<Option>? Options { get; set; }

    }
}

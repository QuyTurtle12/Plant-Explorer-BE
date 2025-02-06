using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class BugReport : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Context {  get; set; }

        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
    }
}

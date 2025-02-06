using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Avatar : BaseEntity
    {
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<ApplicationUser>? Users { get; set; }
    }
}

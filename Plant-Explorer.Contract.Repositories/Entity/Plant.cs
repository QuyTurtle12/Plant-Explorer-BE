using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Plant : BaseEntity
    {
        public string? ScientificName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation Properties
        public virtual ICollection<ScanHistory>? ScanHistories { get; set; }
    }
}

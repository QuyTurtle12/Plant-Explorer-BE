using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Plant : BaseEntity
    {
        public string? ScientificName { get; set; }
        public string? Family { get; set; }
        public string? Description { get; set; }
        public string? Habitat { get; set; }
        public string? Distribution { get; set; }

        // Navigation Properties
        public virtual ICollection<ScanHistory>? ScanHistories { get; set; }
        public virtual ICollection<FavoritePlant>? FavoritePlants { get; set; }
        public virtual ICollection<PlantCharacteristic>? PlantCharacteristics { get; set; }
        public virtual ICollection<PlantApplication>? PlantApplications { get; set; }
        public virtual ICollection<PlantImage>? PlantImages { get; set; }
    }
}

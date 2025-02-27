using Plant_Explorer.Contract.Repositories.Entity;

namespace Plant_Explorer.Contract.Repositories
{
    public class PlantImage
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public string? ImageUrl { get; set; }

        public PlantImage() 
        {
            Id = Guid.NewGuid();
        }

        // Navigation Properties
        public virtual Plant? Plant { get; set; }
    }
}

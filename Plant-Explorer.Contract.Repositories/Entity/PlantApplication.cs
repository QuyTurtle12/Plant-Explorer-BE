namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class PlantApplication
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public Guid ApplicationCategoryId { get; set; }
        public string? Description { get; set; }

        public PlantApplication()
        {
            Id = Guid.NewGuid();
        }

        // Navigation Properties
        public virtual Plant? Plant { get; set; }
        public virtual ApplicationCategory? ApplicationCategory { get; set; }
    }
}

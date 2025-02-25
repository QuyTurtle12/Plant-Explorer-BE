namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class FavoritePlant
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PlantId { get; set; }

        public FavoritePlant() 
        {
            Id = Guid.NewGuid();
        }

        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Plant? Plant { get; set; }
    }
}

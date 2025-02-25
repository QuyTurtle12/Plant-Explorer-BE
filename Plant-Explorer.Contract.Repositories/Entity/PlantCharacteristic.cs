using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class PlantCharacteristic
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public Guid CharacteristicCategoryId { get; set; }
        public string? Description { get; set; }

        public PlantCharacteristic() 
        {
            Id = Guid.NewGuid();
        }

        // Navigation Properties
        public virtual Plant? Plant { get; set; }
        public virtual CharacteristicCategory? CharacteristicCategory { get; set; }
    }
}

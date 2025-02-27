using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class CharacteristicCategory : BaseEntity
    {
        // Navigation Properties
        public virtual ICollection<PlantCharacteristic>? PlantCharacteristics { get; set; }
    }
}

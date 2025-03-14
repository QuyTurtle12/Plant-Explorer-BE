namespace Plant_Explorer.Contract.Repositories.ModelViews
{
    public class PlantCharacteristicGetModel
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public Guid CharacteristicCategoryId { get; set; }
        public string? Description { get; set; }
        //Show name for UI
        public string? PlantName { get; set; }
        public string? CharacteristicName { get; set; }
    }

    public class PlantCharacteristicPostModel
    {
        public Guid PlantId { get; set; }
        public Guid CharacteristicCategoryId { get; set; }
        public string? Description { get; set; }
    }

    public class PlantCharacteristicPutModel
    {
        public string? Description { get; set; }
    }

}

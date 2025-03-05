namespace Plant_Explorer.Contract.Repositories.ModelViews
{
    public class BasePlantModel
    {
        public string? ScientificName { get; set; }
        public string? Family { get; set; }
        public string? Description { get; set; }
        public string? Habitat { get; set; }
        public string? Distribution { get; set; }
    }

    public class PlantGetModel : BasePlantModel
    {
        public Guid Id { get; set; }
    }

    public class PlantPostModel : BasePlantModel
    {
        public new string ScientificName { get; set; } = string.Empty;
        public new string Family { get; set; } = string.Empty;
        public new string Description { get; set; } = string.Empty;
        public new string Habitat { get; set; } = string.Empty;
        public new string Distribution { get; set; } = string.Empty;
    }

    public class PlantPutModel : BasePlantModel
    {
    }
}

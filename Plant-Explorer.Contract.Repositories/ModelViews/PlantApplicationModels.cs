namespace Plant_Explorer.Contract.Repositories.ModelViews
{
    public class PlantApplicationGetModel
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public Guid ApplicationCategoryId { get; set; }
        public string? Description { get; set; }
        public string? PlantName { get; set; }
        public string? ApplicationCategoryName { get; set; }
    }

    public class PlantApplicationPostModel
    {
        public Guid PlantId { get; set; }
        public Guid ApplicationCategoryId { get; set; }
        public string? Description { get; set; }
    }

    public class PlantApplicationPutModel
    {
        public string? Description { get; set; }
    }
}

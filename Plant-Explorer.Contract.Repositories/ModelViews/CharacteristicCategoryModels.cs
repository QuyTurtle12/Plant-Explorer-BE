namespace Plant_Explorer.Contract.Repositories.ModelViews
{
    public class CharacteristicCategoryGetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CharacteristicCategoryPostModel
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CharacteristicCategoryPutModel
    {
        public string Name { get; set; } = string.Empty;
    }
}

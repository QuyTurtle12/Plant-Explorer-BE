namespace Plant_Explorer.Contract.Repositories.ModelViews
{
    public class ApplicationCategoryModels
    {
        public class ApplicationCategoryGetModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int? Status { get; set; }
        }

        public class ApplicationCategoryPostModel
        {
            public string Name { get; set; } = string.Empty;
        }

        public class ApplicationCategoryPutModel
        {
            public string Name { get; set; } = string.Empty;
        }

    }
}

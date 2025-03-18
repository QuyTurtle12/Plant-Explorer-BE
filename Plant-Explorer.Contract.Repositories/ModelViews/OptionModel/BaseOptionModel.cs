namespace Plant_Explorer.Contract.Repositories.ModelViews.OptionModel
{

    public class BaseOptionModel
    {
        //public string Name { get; set; } = string.Empty;
        public Guid QuestionId { get; set; }
        public string? Context { get; set; }
        public bool IsCorrect { get; set; }
    }
}


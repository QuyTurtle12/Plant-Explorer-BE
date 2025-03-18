namespace Plant_Explorer.Contract.Repositories.ModelViews.OptionModel
{
    public class PostOptionModel : BaseOptionModel 
    {
        public Guid QuestionId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

using Plant_Explorer.Contract.Repositories.Base;



namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Option : BaseEntity
    {
        public Guid QuestionId { get; set; }
        public string Context { get; set; }
        public bool IsCorrect { get; set; }
       

        // Navigation Properties
        public virtual Question Question { get; set; }
    }
}
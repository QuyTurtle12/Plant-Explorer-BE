using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class ApplicationCategory : BaseEntity
    {
        // Navigation Properties
        public virtual ICollection<PlantApplication>? PlantApplications { get; set; }
        public virtual ICollection<Quiz>? Quizzes { get; set; }
    }
}

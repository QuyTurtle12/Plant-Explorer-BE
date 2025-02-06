using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class UserPoint : BaseEntity
    {
        public Guid UserId { get; set; }
        public int? Rank { get; set; }
        public int Point { get; set; }

        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public UserPoint() {
            Point = 0;
        }
    }
}

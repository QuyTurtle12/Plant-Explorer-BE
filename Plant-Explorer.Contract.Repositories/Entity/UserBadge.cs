namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class UserBadge
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid BadgeId { get; set; }
        public DateTime DateEarned { get; set; }

        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Badge? Badge { get; set; }

        public UserBadge() 
        {
            Id = Guid.NewGuid();

        }
    }
}

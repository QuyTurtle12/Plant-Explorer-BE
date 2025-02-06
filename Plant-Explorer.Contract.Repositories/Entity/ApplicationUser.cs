using Microsoft.AspNetCore.Identity;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public int? Age { get; set; }
        public Guid RoleId { get; set; }
        public Guid? AvatarId { get; set; }
        public int? Status { get; set; } = 1;
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        //Navigation Properties
        public virtual ApplicationRole? Role { get; set; }
        public virtual ICollection<BugReport>? BugReports { get; set; }
        public virtual ICollection<ScanHistory>? ScanHistories { get; set; }
        public virtual Avatar? Avatar { get; set; }
        public virtual UserPoint? UserPoint { get; set; }
        public virtual ICollection<UserBadge>? UserBadges { get; set; }
        public virtual ICollection<QuizAttempt>? QuizAttempts { get; set; }

        public ApplicationUser() 
        {
            Id = Guid.NewGuid();
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
            EmailConfirmed = false;
        }
    }
}

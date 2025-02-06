using Microsoft.AspNetCore.Identity;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        // Navigation property
        public virtual ICollection<ApplicationUser>? Users { get; set; }
        
        public ApplicationRole()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}

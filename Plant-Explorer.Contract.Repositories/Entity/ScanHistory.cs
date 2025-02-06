using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class ScanHistory
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PlantId { get; set; }
        public DateTime ScanTime { get; set; }

        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Plant? Plant { get; set; }

        public ScanHistory() {
            Id = Guid.NewGuid();
            ScanTime = CoreHelper.SystemTimeNow;
        }
    }
}

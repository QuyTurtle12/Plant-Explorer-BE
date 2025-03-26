using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Contract.Repositories.Base
{
    public abstract class BaseEntity
    {

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedTime = LastUpdatedTime = CoreHelper.SystemTimeNow;
        }

        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public int? Status { get; set; } = 1;
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset? CreatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }

        [NotMapped]
        private bool IsDisposed { get; set; }

        
    }
}

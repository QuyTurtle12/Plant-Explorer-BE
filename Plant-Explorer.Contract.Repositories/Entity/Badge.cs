﻿using Plant_Explorer.Contract.Repositories.Base;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Badge : BaseEntity
    {
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int? conditionalPoint { get; set; }
        public string? imageUrl { get; set; }

        // Navigation Properties
        public virtual ICollection<UserBadge>? UserBadges { get; set; }
    }
}
using Plant_Explorer.Contract.Repositories.Base;
using System.Diagnostics.Contracts;

using Plant_Explorer.Contract.Repositories.Base;
using System;
using System.Collections.Generic;

namespace Plant_Explorer.Contract.Repositories.Entity
{
    public class Quiz : BaseEntity
    {
        public string? ImageUrl { get; set; }
        public int? Point { get; set; }
        public string? Context { get; set; }

        // Navigation Properties
        public virtual ICollection<Question>? Questions { get; set; }
        public virtual ICollection<QuizAttempt>? QuizAttempts { get; set; }
    }
}
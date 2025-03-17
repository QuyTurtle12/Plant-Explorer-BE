using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.OptionModel
{
    public class PostOptionModel : BaseOptionModel {
        public Guid Id { get; set; } // Có thể là Guid
        public Guid QuestionId { get; set; } // Có thể là Guid
        public string Name { get; set; }
    }
}

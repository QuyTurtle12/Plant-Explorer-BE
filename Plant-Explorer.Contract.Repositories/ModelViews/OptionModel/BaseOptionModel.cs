using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.OptionModel
{

    public class BaseOptionModel
    {
        public string Name { get; set; } = string.Empty;
        public string? QuestionId { get; set; }
        public string? Context { get; set; }
        public bool IsCorrect { get; set; }
    }
}


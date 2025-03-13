using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.QuestionModel
{
    public class PutQuestionModel : BaseQuestionModel
    {
        public string? Content { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}

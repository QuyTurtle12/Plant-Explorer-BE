using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.Quiz
{
    public class UpdateQuizDto
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Status { get; set; } 
    }

}

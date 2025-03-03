using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Repositories.ModelViews.PlantImageModel
{
    public class PlantImageResponse
    {
        public Guid Id { get; set; }
        public Guid PlantId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}

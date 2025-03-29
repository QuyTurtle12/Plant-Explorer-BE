using Plant_Explorer.Contract.Repositories.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IPlantCharacteristicService
    {
        Task<IEnumerable<PlantCharacteristicGetModel>> GetAllCharacteristicsAsync();
        Task<IEnumerable<PlantCharacteristicGetModel>> GetCharacteristicByIdAsync(Guid id);
        Task<PlantCharacteristicGetModel> CreateCharacteristicAsync(PlantCharacteristicPostModel model);
        Task<PlantCharacteristicGetModel?> UpdateCharacteristicAsync(Guid id, PlantCharacteristicPutModel model);
        Task<bool> DeleteCharacteristicAsync(Guid id);
    }
}

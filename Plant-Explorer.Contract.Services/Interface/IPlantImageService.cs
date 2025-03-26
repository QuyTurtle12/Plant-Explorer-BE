using Plant_Explorer.Contract.Repositories.ModelViews.PlantImageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IPlantImageService
    {
        Task<PlantImageResponse> CreatePlantImageAsync(CreatePlantImageRequest request);
        Task<PlantImageResponse> UpdatePlantImageAsync(UpdatePlantImageRequest request);
        Task DeletePlantImageAsync(Guid id);
        Task<PlantImageResponse> GetPlantImageByIdAsync(Guid id);
        Task<IEnumerable<PlantImageResponse>> GetAllPlantImagesAsync();
    }
}

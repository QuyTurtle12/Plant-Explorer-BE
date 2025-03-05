using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.PlantImageModel;
using Plant_Explorer.Contract.Repositories;
using Plant_Explorer.Contract.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Plant_Explorer.Services.Services
{
    public class PlantImageService : IPlantImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlantImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PlantImageResponse> CreatePlantImageAsync(CreatePlantImageRequest request)
        {
            var plantImage = new PlantImage
            {
                PlantId = request.PlantId,
                ImageUrl = request.ImageUrl
            };

            await _unitOfWork.GetRepository<PlantImage>().InsertAsync(plantImage);
            await _unitOfWork.SaveAsync();

            return new PlantImageResponse
            {
                Id = plantImage.Id,
                PlantId = plantImage.PlantId,
                ImageUrl = plantImage.ImageUrl
            };
        }

        public async Task DeletePlantImageAsync(Guid id)
        {
            var repository = _unitOfWork.GetRepository<PlantImage>();
            var plantImage = await repository.Entities.FirstOrDefaultAsync(pi => pi.Id == id);
            if (plantImage == null)
                throw new Exception("Plant image not found");

            await repository.DeleteAsync(plantImage);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<PlantImageResponse>> GetAllPlantImagesAsync()
        {
            var images = await _unitOfWork.GetRepository<PlantImage>().Entities.ToListAsync();
            return images.Select(pi => new PlantImageResponse
            {
                Id = pi.Id,
                PlantId = pi.PlantId,
                ImageUrl = pi.ImageUrl
            });
        }

        public async Task<PlantImageResponse> GetPlantImageByIdAsync(Guid id)
        {
            var pi = await _unitOfWork.GetRepository<PlantImage>().Entities.FirstOrDefaultAsync(pi => pi.Id == id);
            if (pi == null)
                throw new Exception("Plant image not found");

            return new PlantImageResponse
            {
                Id = pi.Id,
                PlantId = pi.PlantId,
                ImageUrl = pi.ImageUrl
            };
        }

        public async Task<PlantImageResponse> UpdatePlantImageAsync(UpdatePlantImageRequest request)
        {
            var repository = _unitOfWork.GetRepository<PlantImage>();
            var pi = await repository.Entities.FirstOrDefaultAsync(pi => pi.Id == request.Id);
            if (pi == null)
                throw new Exception("Plant image not found");

            pi.PlantId = request.PlantId;
            pi.ImageUrl = request.ImageUrl;
            // Since PlantImage entity does not have a Name property, we only return the updated name in the DTO.
            await repository.UpdateAsync(pi);
            await _unitOfWork.SaveAsync();

            return new PlantImageResponse
            {
                Id = pi.Id,
                PlantId = pi.PlantId,
                ImageUrl = pi.ImageUrl
            };
        }
    }
}

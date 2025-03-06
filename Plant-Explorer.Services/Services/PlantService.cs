using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Services.Services
{
    public class PlantService : IPlantService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PlantService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlantGetModel>> GetAllPlantsAsync()
        {
            List<Plant> plantList = (List<Plant>)await _unitOfWork.GetRepository<Plant>().GetAllAsync();
            return _mapper.Map<IEnumerable<PlantGetModel>>(plantList);
        }

        public async Task<PlantGetModel?> GetPlantByIdAsync(Guid id)
        {
            Plant plant = await _unitOfWork.GetRepository<Plant>().GetByIdAsync(id);
            return plant != null ? _mapper.Map<PlantGetModel>(plant) : null;
        }

        public async Task<PlantGetModel> CreatePlantAsync(PlantPostModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ScientificName) || string.IsNullOrWhiteSpace(model.Family))
                throw new ArgumentException("Scientific Name and Family are required");

            Plant plantEntity = _mapper.Map<Plant>(model);
            await _unitOfWork.GetRepository<Plant>().InsertAsync(plantEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PlantGetModel>(plantEntity);
        }

        public async Task<PlantGetModel?> UpdatePlantAsync(Guid id, PlantPutModel model)
        {
            Plant plantEntity = await _unitOfWork.GetRepository<Plant>().GetByIdAsync(id);
            if (plantEntity == null)
                return null;

            if (!string.IsNullOrWhiteSpace(model.ScientificName))
                plantEntity.ScientificName = model.ScientificName;
            if (!string.IsNullOrWhiteSpace(model.Family))
                plantEntity.Family = model.Family;
            if (!string.IsNullOrWhiteSpace(model.Description))
                plantEntity.Description = model.Description;
            if (!string.IsNullOrWhiteSpace(model.Habitat))
                plantEntity.Habitat = model.Habitat;
            if (!string.IsNullOrWhiteSpace(model.Distribution))
                plantEntity.Distribution = model.Distribution;

            await _unitOfWork.GetRepository<Plant>().UpdateAsync(plantEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PlantGetModel>(plantEntity);
        }

        public async Task<bool> DeletePlantAsync(Guid id)
        {
            Plant plantEntity = await _unitOfWork.GetRepository<Plant>().GetByIdAsync(id);
            if (plantEntity == null)
                return false;

            await _unitOfWork.GetRepository<Plant>().DeleteAsync(plantEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<PlantGetModel?> GetPlantByScientificName(string scientificName)
        {
            Plant? plant = await _unitOfWork.GetRepository<Plant>().Entities
                .Where(p => p.ScientificName == scientificName || p.ScientificName.Equals(scientificName))
                .FirstOrDefaultAsync();

            return plant != null ? _mapper.Map<PlantGetModel>(plant) : null;
        }
        public async Task<IEnumerable<PlantGetModel>> SearchPlantsByName(string searchStringName)
        {
            List<Plant> plantList = await _unitOfWork.GetRepository<Plant>().Entities
                .Where(p => p.Name.ToLower().Contains(searchStringName.ToLower())
                || p.ScientificName.ToLower().Contains(searchStringName.ToLower()))
                .ToListAsync();

            return _mapper.Map<IEnumerable<PlantGetModel>>(plantList);
        }

        public async Task<bool> SoftDeletePlantAsync(Guid id)
        {
            Plant plantEntity = await _unitOfWork.GetRepository<Plant>().GetByIdAsync(id);
            if (plantEntity == null)
                return false;
            //Soft delete
            plantEntity.Status = 0;
            plantEntity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            plantEntity.DeletedTime = plantEntity.LastUpdatedTime;

            await _unitOfWork.GetRepository<Plant>().UpdateAsync(plantEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}

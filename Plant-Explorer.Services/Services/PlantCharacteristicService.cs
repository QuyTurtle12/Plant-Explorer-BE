using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Services.Services
{
    public class PlantCharacteristicService : IPlantCharacteristicService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PlantCharacteristicService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlantCharacteristicGetModel>> GetAllCharacteristicsAsync()
        {
            List<PlantCharacteristic> characteristics = (List<PlantCharacteristic>) await _unitOfWork.GetRepository<PlantCharacteristic>().GetAllAsync();

            List<PlantCharacteristicGetModel> result = (List<PlantCharacteristicGetModel>)_mapper.Map<IEnumerable<PlantCharacteristicGetModel>>(characteristics);

            foreach(var item in result)
            {
                await AssignPlantNameCharacteristicNameToGetModel(item);
            }

            return result;
        }
        public async Task<PlantCharacteristicGetModel?> GetCharacteristicByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid characteristic ID");

            PlantCharacteristic characteristic = await _unitOfWork.GetRepository<PlantCharacteristic>().GetByIdAsync(id);


            PlantCharacteristicGetModel result = _mapper.Map<PlantCharacteristicGetModel>(characteristic);
            await AssignPlantNameCharacteristicNameToGetModel(result);

            return result;
        }

        public async Task<PlantCharacteristicGetModel> CreateCharacteristicAsync(PlantCharacteristicPostModel model)
        {
            if (model.PlantId == Guid.Empty || model.CharacteristicCategoryId == Guid.Empty)
                throw new ArgumentException("PlantId and CharacteristicCategoryId are required");

            PlantCharacteristic characteristicEntity = _mapper.Map<PlantCharacteristic>(model);

            await _unitOfWork.GetRepository<PlantCharacteristic>().InsertAsync(characteristicEntity);

            await _unitOfWork.SaveAsync();

            PlantCharacteristicGetModel result = _mapper.Map<PlantCharacteristicGetModel>(characteristicEntity);
            await AssignPlantNameCharacteristicNameToGetModel(result);
            return result;
        }

        public async Task<PlantCharacteristicGetModel?> UpdateCharacteristicAsync(Guid id, PlantCharacteristicPutModel model)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid characteristic ID");

            PlantCharacteristic characteristicEntity = await _unitOfWork.GetRepository<PlantCharacteristic>().GetByIdAsync(id);
            if (characteristicEntity == null)
                return null;

            if (!string.IsNullOrWhiteSpace(model.Description))
                characteristicEntity.Description = model.Description;

            _unitOfWork.GetRepository<PlantCharacteristic>().Update(characteristicEntity);
            await _unitOfWork.SaveAsync();

            PlantCharacteristicGetModel result = _mapper.Map<PlantCharacteristicGetModel>(characteristicEntity);
            await AssignPlantNameCharacteristicNameToGetModel(result);
            return result;
        }

        public async Task<bool> DeleteCharacteristicAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid characteristic ID");

            PlantCharacteristic characteristicEntity = await _unitOfWork.GetRepository<PlantCharacteristic>().GetByIdAsync(id);
            if (characteristicEntity == null)
                return false;

            _unitOfWork.GetRepository<PlantCharacteristic>().Delete(characteristicEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
        private async Task AssignPlantNameCharacteristicNameToGetModel(PlantCharacteristicGetModel result)
        {
            result.PlantName = await _unitOfWork.GetRepository<Plant>()
                    .Entities
                    .Where(p => p.Id == result.PlantId)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();
            result.CharacteristicName = await _unitOfWork.GetRepository<CharacteristicCategory>()
                    .Entities
                    .Where(c => c.Id == result.CharacteristicCategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
        }
    }

}

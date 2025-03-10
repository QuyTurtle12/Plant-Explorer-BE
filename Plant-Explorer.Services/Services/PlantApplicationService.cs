using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
using static System.Net.Mime.MediaTypeNames;

namespace Plant_Explorer.Services.Services
{
    public class PlantApplicationService : IPlantApplicationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PlantApplicationService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlantApplicationGetModel>> GetAllAsync()
        {
            List<PlantApplication> applications = (List<PlantApplication>) await _unitOfWork.GetRepository<PlantApplication>().GetAllAsync();

            List<PlantApplicationGetModel> result = (List<PlantApplicationGetModel>) _mapper.Map<IEnumerable<PlantApplicationGetModel>>(applications);

            foreach (PlantApplicationGetModel plantGetModel in result)
            {
                await AssignPlantNameApplicationNameToGetModel(plantGetModel);
            }
            return result;
        }

        public async Task<PlantApplicationGetModel?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID");

            PlantApplication application = await _unitOfWork.GetRepository<PlantApplication>().GetByIdAsync(id);

            PlantApplicationGetModel result = _mapper.Map<PlantApplicationGetModel>(application);
            await AssignPlantNameApplicationNameToGetModel(result);
            return result;

        }

        public async Task<PlantApplicationGetModel> CreateAsync(PlantApplicationPostModel model)
        {
            if(model.PlantId == Guid.Empty || model.ApplicationCategoryId == Guid.Empty)
                throw new ArgumentException("Invalid plant id or application category id");

            PlantApplication entity = _mapper.Map<PlantApplication>(model);
            await _unitOfWork.GetRepository<PlantApplication>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            PlantApplicationGetModel result = _mapper.Map<PlantApplicationGetModel>(entity);
            await AssignPlantNameApplicationNameToGetModel(result);
            return result;
        }

        public async Task<PlantApplicationGetModel?> UpdateAsync(Guid id, PlantApplicationPutModel model)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID");

            PlantApplication entity = await _unitOfWork.GetRepository<PlantApplication>().GetByIdAsync(id);
            if (entity == null)
                return null;

            entity.Description = model.Description ?? entity.Description;

            _unitOfWork.GetRepository<PlantApplication>().Update(entity);
            await _unitOfWork.SaveAsync();

            PlantApplicationGetModel result = _mapper.Map<PlantApplicationGetModel>(entity);
            await AssignPlantNameApplicationNameToGetModel(result);
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID");

            PlantApplication entity = await _unitOfWork.GetRepository<PlantApplication>().GetByIdAsync(id);
            if (entity == null)
                return false;

            _unitOfWork.GetRepository<PlantApplication>().Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private async Task AssignPlantNameApplicationNameToGetModel(PlantApplicationGetModel plantGetModel)
        {
            plantGetModel.PlantName = await _unitOfWork.GetRepository<Plant>()
                .Entities
                .Where(p => p.Id == plantGetModel.PlantId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
            plantGetModel.ApplicationCategoryName = await _unitOfWork.GetRepository<ApplicationCategory>()
                .Entities
                .Where(ac => ac.Id == plantGetModel.ApplicationCategoryId)
                .Select(ac => ac.Name)
                .FirstOrDefaultAsync();
        }
    }
}

using AutoMapper;
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
            List<PlantApplication> applications = (List<PlantApplication>)await _unitOfWork.GetRepository<PlantApplication>().GetAllAsync();
            return _mapper.Map<IEnumerable<PlantApplicationGetModel>>(applications);
        }

        public async Task<PlantApplicationGetModel?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID");

            PlantApplication application = await _unitOfWork.GetRepository<PlantApplication>().GetByIdAsync(id);
            return application != null ? _mapper.Map<PlantApplicationGetModel>(application) : null;
        }

        public async Task<PlantApplicationGetModel> CreateAsync(PlantApplicationPostModel model)
        {
            PlantApplication entity = _mapper.Map<PlantApplication>(model);
            await _unitOfWork.GetRepository<PlantApplication>().InsertAsync(entity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<PlantApplicationGetModel>(entity);
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
            return _mapper.Map<PlantApplicationGetModel>(entity);
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
    }
}

using AutoMapper;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Utils;
using static Plant_Explorer.Contract.Repositories.ModelViews.ApplicationCategoryModels;

namespace Plant_Explorer.Services.Services
{
    public class ApplicationCategoryService : IApplicationCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationCategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ApplicationCategoryGetModel>> GetAllCategoriesAsync()
        {
            List<ApplicationCategory> categories = (List<ApplicationCategory>)await _unitOfWork.GetRepository<ApplicationCategory>().GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicationCategoryGetModel>>(categories);
        }

        public async Task<ApplicationCategoryGetModel?> GetCategoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            ApplicationCategory category = await _unitOfWork.GetRepository<ApplicationCategory>().GetByIdAsync(id);
            return category != null ? _mapper.Map<ApplicationCategoryGetModel>(category) : null;
        }

        public async Task<ApplicationCategoryGetModel> CreateCategoryAsync(ApplicationCategoryPostModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name is required");

            ApplicationCategory categoryEntity = _mapper.Map<ApplicationCategory>(model);
            await _unitOfWork.GetRepository<ApplicationCategory>().InsertAsync(categoryEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ApplicationCategoryGetModel>(categoryEntity);
        }

        public async Task<ApplicationCategoryGetModel?> UpdateCategoryAsync(Guid id, ApplicationCategoryPutModel model)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            ApplicationCategory categoryEntity = await _unitOfWork.GetRepository<ApplicationCategory>().GetByIdAsync(id);
            if (categoryEntity == null)
                return null;

            if (!string.IsNullOrWhiteSpace(model.Name))
                categoryEntity.Name = model.Name;

            _unitOfWork.GetRepository<ApplicationCategory>().Update(categoryEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ApplicationCategoryGetModel>(categoryEntity);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            ApplicationCategory categoryEntity = await _unitOfWork.GetRepository<ApplicationCategory>().GetByIdAsync(id);
            if (categoryEntity == null)
                return false;

            _unitOfWork.GetRepository<ApplicationCategory>().Delete(categoryEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> SoftDeleteApplicationCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            ApplicationCategory categoryEntity = await _unitOfWork.GetRepository<ApplicationCategory>().GetByIdAsync(id);
            if (categoryEntity == null)
                return false;

            categoryEntity.Status = 0;
            categoryEntity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            categoryEntity.DeletedTime = categoryEntity.LastUpdatedTime;

            _unitOfWork.GetRepository<ApplicationCategory>().Update(categoryEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}

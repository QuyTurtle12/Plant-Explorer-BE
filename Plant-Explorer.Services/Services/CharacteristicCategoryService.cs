using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Services.Services
{
    public class CharacteristicCategoryService : ICharacteristicCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CharacteristicCategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CharacteristicCategoryGetModel>> GetAllCategoriesAsync()
        {
            List<CharacteristicCategory> categories = await _unitOfWork.GetRepository<CharacteristicCategory>()
                .Entities
                .Where(c => c.DeletedTime == null)
                .ToListAsync();
            return _mapper.Map<IEnumerable<CharacteristicCategoryGetModel>>(categories);
        }

        public async Task<CharacteristicCategoryGetModel?> GetCategoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            CharacteristicCategory? category = await _unitOfWork.GetRepository<CharacteristicCategory>()
                .Entities
                .Where(c => c.Id == id && c.DeletedTime == null)
                .FirstOrDefaultAsync();

            return category != null ? _mapper.Map<CharacteristicCategoryGetModel>(category) : null;
        }

        public async Task<CharacteristicCategoryGetModel> CreateCategoryAsync(CharacteristicCategoryPostModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new ArgumentException("Name is required");

            CharacteristicCategory categoryEntity = _mapper.Map<CharacteristicCategory>(model);
            await _unitOfWork.GetRepository<CharacteristicCategory>().InsertAsync(categoryEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<CharacteristicCategoryGetModel>(categoryEntity);
        }

        public async Task<CharacteristicCategoryGetModel?> UpdateCategoryAsync(Guid id, CharacteristicCategoryPutModel model)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            CharacteristicCategory? categoryEntity = await _unitOfWork.GetRepository<CharacteristicCategory>().GetByIdAsync(id);
            if (categoryEntity == null || categoryEntity.DeletedTime != null)
                return null;

            if (!string.IsNullOrWhiteSpace(model.Name))
                categoryEntity.Name = model.Name;

            await _unitOfWork.GetRepository<CharacteristicCategory>().UpdateAsync(categoryEntity);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<CharacteristicCategoryGetModel>(categoryEntity);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            CharacteristicCategory? categoryEntity = await _unitOfWork.GetRepository<CharacteristicCategory>().GetByIdAsync(id);
            if (categoryEntity == null || categoryEntity.DeletedTime != null)
            {
                Console.WriteLine("Category not found");
                return false;
            }
            if (await IsInUsed(id))
            {
                Console.WriteLine("Category is in used");
                return false;
            }


            await _unitOfWork.GetRepository<CharacteristicCategory>().DeleteAsync(categoryEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> SoftDeleteCharacteristicCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid category ID");

            CharacteristicCategory? categoryEntity = await _unitOfWork.GetRepository<CharacteristicCategory>().GetByIdAsync(id);
            if (categoryEntity == null || categoryEntity.DeletedTime != null)
            {
                Console.WriteLine("Category not found");
                return false;
            }
            if(await IsInUsed(id))
            {
                Console.WriteLine("Category is in used");
                return false;
            }

            // Soft delete
            categoryEntity.Status = 0;
            categoryEntity.LastUpdatedTime = CoreHelper.SystemTimeNow;
            categoryEntity.DeletedTime = categoryEntity.LastUpdatedTime;
            
            await _unitOfWork.GetRepository<CharacteristicCategory>().UpdateAsync(categoryEntity);
            await _unitOfWork.SaveAsync();
            return true;
        }
        private async Task<bool> IsInUsed(Guid id)
        {
            return await _unitOfWork.GetRepository<PlantCharacteristic>().Entities
                .AnyAsync(pc => pc.CharacteristicCategoryId == id);
        }
    }
}

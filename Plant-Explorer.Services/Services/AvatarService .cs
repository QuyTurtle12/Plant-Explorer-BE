using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.AvatarModel;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Services.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AvatarService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<AvatarResponse> CreateAvatarAsync(CreateAvatarRequest request)
        {
            var avatar = new Avatar
            {
                Name = request.Name,
                ImageUrl = request.ImageUrl
            };

            await _unitOfWork.GetRepository<Avatar>().InsertAsync(avatar);
            await _unitOfWork.SaveAsync();

            return new AvatarResponse
            {
                Id = avatar.Id,
                Name = avatar.Name,
                ImageUrl = avatar.ImageUrl
            };
        }

        public async Task DeleteAvatarAsync(Guid id)
        {
            var repository = _unitOfWork.GetRepository<Avatar>();
            var avatar = await repository.Entities.FirstOrDefaultAsync(a => a.Id == id);
            if (avatar == null)
                throw new Exception("Avatar not found");

            await repository.DeleteAsync(avatar);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<AvatarResponse>> GetAllAvatarsAsync()
        {
            var avatars = await _unitOfWork.GetRepository<Avatar>().Entities.ToListAsync();
            return avatars.Select(a => new AvatarResponse
            {
                Id = a.Id,
                Name = a.Name,
                ImageUrl = a.ImageUrl
            });
        }

        public async Task<AvatarResponse> GetAvatarByIdAsync(Guid id)
        {
            var avatar = await _unitOfWork.GetRepository<Avatar>().Entities.FirstOrDefaultAsync(a => a.Id == id);
            if (avatar == null)
                throw new Exception("Avatar not found");

            return new AvatarResponse
            {
                Id = avatar.Id,
                Name = avatar.Name,
                ImageUrl = avatar.ImageUrl
            };
        }

        public async Task<AvatarResponse> UpdateAvatarAsync(UpdateAvatarRequest request)
        {
            var repository = _unitOfWork.GetRepository<Avatar>();
            var avatar = await repository.Entities.FirstOrDefaultAsync(a => a.Id == request.Id);
            if (avatar == null)
                throw new Exception("Avatar not found");

            avatar.Name = request.Name;
            avatar.ImageUrl = request.ImageUrl;
            avatar.LastUpdatedTime = DateTimeOffset.UtcNow;

            await repository.UpdateAsync(avatar);
            await _unitOfWork.SaveAsync();

            return new AvatarResponse
            {
                Id = avatar.Id,
                Name = avatar.Name,
                ImageUrl = avatar.ImageUrl
            };
        }

        public async Task UpdateUserAvatarAsync(Guid avatarId)
        {
            var repository = _unitOfWork.GetRepository<Avatar>();
            var avatar = await repository.Entities.FirstOrDefaultAsync(a => a.Id == avatarId);
            if (avatar == null) throw new Exception("Avatar not found");

            string currentUserId = _tokenService.GetCurrentUserId();

            ApplicationUser? user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(currentUserId);

            if (user == null) throw new Exception("User not found");

            user.AvatarId = avatarId;

            await _unitOfWork.GetRepository<ApplicationUser>().UpdateAsync(user);
            await _unitOfWork.SaveAsync();
        }
    }

}

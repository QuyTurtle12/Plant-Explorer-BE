using Plant_Explorer.Contract.Repositories.ModelViews.AvatarModel;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IAvatarService
    {
        Task<AvatarResponse> CreateAvatarAsync(CreateAvatarRequest request);
        Task<AvatarResponse> UpdateAvatarAsync(UpdateAvatarRequest request);
        Task DeleteAvatarAsync(Guid id);
        Task<AvatarResponse> GetAvatarByIdAsync(Guid id);
        Task<IEnumerable<AvatarResponse>> GetAllAvatarsAsync();
        Task UpdateUserAvatarAsync(Guid avatarId);
    }
}

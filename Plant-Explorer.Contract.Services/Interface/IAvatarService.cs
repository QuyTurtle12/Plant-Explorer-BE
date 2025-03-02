using Plant_Explorer.Contract.Repositories.ModelViews.AvatarModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IAvatarService
    {
        Task<AvatarResponse> CreateAvatarAsync(CreateAvatarRequest request);
        Task<AvatarResponse> UpdateAvatarAsync(UpdateAvatarRequest request);
        Task DeleteAvatarAsync(Guid id);
        Task<AvatarResponse> GetAvatarByIdAsync(Guid id);
        Task<IEnumerable<AvatarResponse>> GetAllAvatarsAsync();
    }
}

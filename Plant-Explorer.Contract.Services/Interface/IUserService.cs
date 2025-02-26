using Plant_Explorer.Contract.Repositories.ModelViews.UserModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Core.Constants.Enum.EnumUser;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IUserService
    {
        Task<PaginatedList<GetUserModel>> GetAllUsersAsync(int index, int pageSize, string? idSearch, string? nameSearch, EnumRole? role);
        Task<GetUserModel> GetUserProfileAsync(string id);
        Task CreateUserAsync(PostUserModel newUser);
        Task UpdateUserAsync(string id, PutUserModel user);
        Task DeleteUserAsync(string id);
    }
}

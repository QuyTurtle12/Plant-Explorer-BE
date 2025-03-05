using Plant_Explorer.Contract.Repositories.ModelViews.RoleModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IRoleService
    {
        Task<PaginatedList<GetRoleModel>> GetAllRolesAsync(int index, int pageSize, string? idSearch, string? nameSearch);
    }
}

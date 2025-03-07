using Plant_Explorer.Contract.Repositories.ModelViews.OptionModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IOptionService
    {
        Task<PaginatedList<GetOptionModel>> GetAllOptionsAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? questionId);
        Task<GetOptionModel> GetOptionByIdAsync(string id);
        Task CreateOptionAsync(PostOptionModel newOption);
        Task UpdateOptionAsync(string id, PutOptionModel updatedOption);
        Task DeleteOptionAsync(string id);
    }
}

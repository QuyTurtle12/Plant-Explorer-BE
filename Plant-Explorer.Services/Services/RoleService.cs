using AutoMapper;
using Microsoft.AspNetCore.Http;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.RoleModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;

namespace Plant_Explorer.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<GetRoleModel>> GetAllRolesAsync(int index, int pageSize, string? idSearch, string? nameSearch)
        {
            // index checking
            if (index <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "index need to be bigger than 0");
            }

            // pageSize checking
            if (pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "pageSize need to be bigger than 0");
            }

            // Get list of user
            IQueryable<ApplicationRole> query = _unitOfWork.GetRepository<ApplicationRole>().Entities;

            // Check if user want to search a user by userId
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                // Convert to guid
                Guid.TryParse(idSearch, out Guid id);

                // Search user by id
                query = query.Where(u => u.Id.Equals(id));
            }

            // Check if user want to search users by name 
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                // Search user by name
                query = query.Where(u => u.Name!.Contains(nameSearch));
            }

            // Skip deleted item
            query = query.Where(u => !u.DeletedTime.HasValue);

            // Sort the list by name
            query = query.OrderBy(u => u.Name);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<ApplicationRole> resultQuery = await _unitOfWork.GetRepository<ApplicationRole>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetRoleModel> responseItem = resultQuery.Items.Select(item =>
            {
                // Map ApplicationRole to ModelView in order to filter unnecessary data 
                GetRoleModel model = _mapper.Map<GetRoleModel>(item);

                model.CreatedTime = item.CreatedTime.ToString("dd-MM-yyyy");
                model.LastUpdatedTime = item.LastUpdatedTime.ToString("dd-MM-yyyy");

                return model;
            }

            ).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetRoleModel> paginatedList = new(
                responseItem,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.PageSize
                );

            return paginatedList;
        }
    }
}

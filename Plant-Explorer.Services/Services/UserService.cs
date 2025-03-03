using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.UserModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.Constants.Enum.EnumUser;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Services.Infrastructure;

namespace Plant_Explorer.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public const string children = "Children";
        public const string staff = "Staff";
        public const string admin = "Admin";

        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<PaginatedList<GetUserModel>> GetAllUsersAsync(int index, int pageSize, string? idSearch, string? nameSearch, EnumRole? role)
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
            IQueryable<ApplicationUser> query = _unitOfWork.GetRepository<ApplicationUser>().Entities;

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
                query = query.Where(u => u.Name.Contains(nameSearch));
            }

            // Check if user want to get a list of user base on their role
            switch (role)
            {
                // Get list of children
                case EnumRole.Children:
                    ApplicationRole? childrenRole = await _unitOfWork.GetRepository<ApplicationRole>().Entities
                                                        .Where(r => r.Name!.Equals(children) && !r.DeletedTime.HasValue)
                                                        .FirstOrDefaultAsync();
                    if (childrenRole != null)
                    {
                        Guid roleId = childrenRole.Id;

                        query = query.Where(u => u.RoleId.Equals(roleId));
                    }
                    break;
                // Get list of staff
                case EnumRole.Staff:
                    ApplicationRole? staffRole = await _unitOfWork.GetRepository<ApplicationRole>().Entities
                                    .Where(r => r.Name!.Equals(staff) && !r.DeletedTime.HasValue)
                                    .FirstOrDefaultAsync();
                    if (staffRole != null)
                    {
                        Guid roleId = staffRole.Id;

                        query = query.Where(u => u.RoleId.Equals(roleId));
                    }
                    break;

                default: break;
            }

            // Skip deleted item
            query = query.Where(u => !u.DeletedTime.HasValue);

            //// Get current login user id
            //string currentUserId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor).ToUpper();

            //// Convert to guid type
            //Guid.TryParse(currentUserId, out Guid userId);

            ////Skip current login account
            //query = query.Where(u => u.Id != userId);

            // Sort the list by name
            query = query.OrderBy(u => u.Name);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<ApplicationUser> resultQuery = await _unitOfWork.GetRepository<ApplicationUser>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetUserModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map ApplicationUser to ModelView in order to filter unnecessary data 
                GetUserModel userModel = _mapper.Map<GetUserModel>(item);

                // Get role name of user
                string? roleName = _unitOfWork.GetRepository<ApplicationRole>().Entities
                                        .Where(r => r.Id.Equals(item.RoleId))
                                        .Select(r => r.Name)
                                        .FirstOrDefault();

                if (roleName != null) userModel.Role = roleName;

                // Format CreatedTime
                userModel.CreatedTime = item.CreatedTime.ToString("dd-MM-yyyy");

                return userModel;
            }).Where(item => item != null).ToList(); // Filter null item and return list

            // Create a new paginated list for response data
            PaginatedList<GetUserModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }
    
        public async Task<string> GetCurrentUserId()
        {
            var user = _contextAccessor.HttpContext?.User;

            string id = user.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            string username = user.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            string role = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            return id;
        }
    }
}

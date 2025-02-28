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
using Plant_Explorer.Core.Utils;
using System.Net.Mail;

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

        public const int INACTIVE = 0;
        public const int ACTIVE = 1;

        public UserService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        public async Task<PaginatedList<GetUserModel>> GetAllUsersAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? emailSearch, EnumRole? role)
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
            
            // Check if user want to search users by email 
            if (!string.IsNullOrWhiteSpace(emailSearch))
            {
                // Search user by email
                query = query.Where(u => u.Email!.Contains(emailSearch));
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

                // Get avatar image of user
                string? avatarImageUrl = _unitOfWork.GetRepository<Avatar>().Entities
                                    .Where(a => a.Id.Equals(item.AvatarId))
                                    .Select(a => a.ImageUrl)
                                    .FirstOrDefault();

                // Assign entities attributes that cannot auto mapping to view model 
                if (roleName != null) userModel.Role = roleName;
                if (avatarImageUrl != null) userModel.AvatarUrl = avatarImageUrl;

                // Format audit fields
                userModel.CreatedTime = item.CreatedTime.ToString("dd-MM-yyyy");
                userModel.LastUpdatedTime = item.LastUpdatedTime.ToString("dd-MM-yyyy");

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

        public async Task<GetUserModel> GetUserProfileAsync(string id)
        {
            ApplicationUser? user = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(Guid.Parse(id)) ;

            // Validate if user is existed
            if (user == null || user.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User not found!");

            GetUserModel result = _mapper.Map<GetUserModel>(user);

            // Get role name of user
            string? roleName = await _unitOfWork.GetRepository<ApplicationRole>().Entities
                                    .Where(r => r.Id.Equals(user.RoleId))
                                    .Select(r => r.Name)
                                    .FirstOrDefaultAsync();

            // Get avatar image of user
            string? avatarImageUrl = await _unitOfWork.GetRepository<Avatar>().Entities
                                .Where(a => a.Id.Equals(user.AvatarId))
                                .Select (a => a.ImageUrl)
                                .FirstOrDefaultAsync();

            // Assign entities attributes that cannot auto mapping to view model 
            if (roleName != null) result.Role = roleName;
            if (avatarImageUrl != null) result.AvatarUrl = avatarImageUrl;

            // Format audit fields
            result.CreatedTime = user.CreatedTime.ToString("dd-MM-yyyy");
            result.LastUpdatedTime = user.LastUpdatedTime.ToString("dd-MM-yyyy");

            return result;
        }

        public async Task CreateUserAsync(PostUserModel newUser)
        {
            // Validate input
            GeneralValidation(newUser);
            MailValidation(newUser);

            // Mapping model to entities
            ApplicationUser user = _mapper.Map<ApplicationUser>(newUser);

            // Get role Id
            Guid? roleId = await _unitOfWork.GetRepository<ApplicationRole>().Entities
                                    .Where(r => r.Name!.Equals(newUser.RoleName))
                                    .Select(r => r.Id)
                                    .FirstOrDefaultAsync();

            // Validate role
            if (!roleId.HasValue) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Invalid Role");

            user.RoleId = roleId.Value;
            
            // Add new user to database and save
            await _unitOfWork.GetRepository<ApplicationUser>().InsertAsync(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateUserAsync(string id, PutUserModel updatedUser)
        {
            // Validate if user existed
            ApplicationUser? existingUser = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                            .Where(u => u.Id.Equals(Guid.Parse(id)))
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This user is not exist!");
            // Validate input
            GeneralValidation(updatedUser);

            // Validate Phone Number Format (if existed)
            if (!string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(updatedUser.PhoneNumber, @"^\d{10,11}$"))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Phone number must contain 10 or 11 digits.");
                }
            }

            // Mapping model to entities
            _mapper.Map(updatedUser, existingUser);

            // Update audit field
            existingUser.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Update user info and save
            _unitOfWork.GetRepository<ApplicationUser>().Update(existingUser);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteUserAsync(string id)
        {
            // Validate if user existed
            ApplicationUser? existingUser = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                            .Where(u => u.Id.Equals(Guid.Parse(id)))
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This user is not exist!");

            // Delete user
            existingUser.Status = INACTIVE;

            // Update audit fields
            existingUser.LastUpdatedTime = CoreHelper.SystemTimeNow;
            existingUser.DeletedTime = existingUser.LastUpdatedTime;

            // Save
            await _unitOfWork.SaveAsync();
        }

        private void GeneralValidation(BaseUserModel user)
        {
            // Validate user's name
            if (string.IsNullOrWhiteSpace(user.Name)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");

            // Validate user's age
            if (user.Age <= 0 || user.Age >= 100) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Age must be between 0 and 100.");

            // Validate Phone Number Format (if existed)
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(user.PhoneNumber, @"^\d{10,11}$"))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Phone number must contain 10 or 11 digits.");
                }
            }
        }

        private void MailValidation(PostUserModel user)
        {
            // Validate user's email
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Email must not be empty!");

            // Mail format checking
            try
            {
                MailAddress? addr = new MailAddress(user.Email);
            }
            catch (FormatException)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Invalid email format.");
            }

            // Mail duplication checking
            bool IsExistingEmail = _unitOfWork.GetRepository<ApplicationUser>().Entities
                .Where(u => u.Email!.Equals(user.Email))
                .Any();

            if (IsExistingEmail) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This email is already existed");
        }
    }
}

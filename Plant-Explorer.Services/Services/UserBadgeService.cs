using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Services.Services
{
    public class UserBadgeService : IUserBadgeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public UserBadgeService(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task CreateUserBadgeAsync(PostUserBadgeModel newUserBadge)
        {
            await GeneralValidationAsync(newUserBadge);

            // Mapping model to entities
            UserBadge badge = _mapper.Map<UserBadge>(newUserBadge);

            // Add date that user receive badge
            badge.DateEarned = CoreHelper.SystemTimeNow;

            // Add new badge to database and save
            await _unitOfWork.GetRepository<UserBadge>().InsertAsync(badge);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetUserBadgeModel>> GetUserBadgesAsync(int index, int pageSize)
        {
            // Get current login user id
            string? userId = _tokenService.GetCurrentUserId();

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

            // user Id checking
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "User Id can not be empty!");
            }


            // Get list of user badge
            IQueryable<UserBadge> query = _unitOfWork.GetRepository<UserBadge>().Entities
                                                    .Where(ub => ub.UserId.Equals(Guid.Parse(userId)));

            // Sort the list by name
            query = query.OrderBy(b => b.DateEarned);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<UserBadge> resultQuery = await _unitOfWork.GetRepository<UserBadge>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetUserBadgeModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map ApplicationUser to ModelView in order to filter unnecessary data 
                GetUserBadgeModel badgeModel = _mapper.Map<GetUserBadgeModel>(item);

                // Get user name
                badgeModel.UserName = _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                    .Where(u => u.Id.Equals(item.UserId))
                                                    .Select(u => u.Name)
                                                    .FirstOrDefault()
                                                    ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This user's name is null");

                // Get badge name
                badgeModel.BadgeName = _unitOfWork.GetRepository<Badge>().Entities
                                    .Where(b => b.Id.Equals(item.BadgeId))
                                    .Select(b => b.Name)
                                    .FirstOrDefault()
                                    ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This badge's name is null");

                // Format DateEarned attribute
                badgeModel.DateEarned = item.DateEarned.ToString("dd-MM-yyyy");

                return badgeModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetUserBadgeModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        private async Task GeneralValidationAsync(PostUserBadgeModel userBadge)
        {
            // Validate user
            if (string.IsNullOrWhiteSpace(userBadge.UserId)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");

            // Validate badge
            if (string.IsNullOrWhiteSpace(userBadge.BadgeId)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Badge must not be empty!");

            // Check user id format
            Guid userIdGuid;
            if (!Guid.TryParse(userBadge.UserId, out userIdGuid))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Invalid User ID format.");
            }

            // Check badge id format
            Guid badgeIdGuid;
            if (!Guid.TryParse(userBadge.BadgeId, out badgeIdGuid))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Invalid Badge ID format.");
            }

            // Validate if user existed
            ApplicationUser? existingUser = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                            .Where(u => u.Id.Equals(Guid.Parse(userBadge.UserId)) && !u.DeletedTime.HasValue)
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This user is not exist!");

            // Validate if badge existed
            Badge? existingBadge = await _unitOfWork.GetRepository<Badge>().Entities
                                                            .Where(b => b.Id.Equals(Guid.Parse(userBadge.BadgeId)) && !b.DeletedTime.HasValue)
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This badge is not exist!");

            // Validate if user has already accquired the badge
            UserBadge? existingUserBadge = await _unitOfWork.GetRepository<UserBadge>().Entities
                                                            .Where(b => b.UserId.Equals(Guid.Parse(userBadge.UserId)) && b.BadgeId.Equals(Guid.Parse(userBadge.BadgeId)))
                                                            .FirstOrDefaultAsync();

            if (existingUserBadge != null) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This badge has already accquired by this user!");
        }
    }
}

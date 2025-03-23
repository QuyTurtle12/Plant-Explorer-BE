using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel;
using Plant_Explorer.Contract.Repositories.ModelViews.UserPointModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Services.Services
{
    public class UserPointService : IUserPointService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserBadgeService _userBadgeService;

        private const int INITIAL_POINT = 0;

        public UserPointService(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService, IUserBadgeService userBadgeService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userBadgeService = userBadgeService;
        }

        public async Task CreateUserPointAsync(PostUserPointModel newUserPoint)
        {
            // Validate if user existed
            ApplicationUser? existingUser = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                            .Where(u => u.Id.Equals(Guid.Parse(newUserPoint.UserId)))
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This user is not exist!");

            // Validate if user point existed
            UserPoint? existingPoint = await _unitOfWork.GetRepository<UserPoint>().Entities
                                                            .Where(u => u.UserId.Equals(Guid.Parse(newUserPoint.UserId)))
                                                            .FirstOrDefaultAsync();

            // print error if user has already had point
            if (existingPoint != null) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This user already has point");

            // Mapping model to entities
            UserPoint userPoint = _mapper.Map<UserPoint>(newUserPoint);
            
            // Initial Point
            userPoint.Point = INITIAL_POINT;

            // Get last rank
            int lastRank = (int)await _unitOfWork.GetRepository<UserPoint>().Entities
                                                .MaxAsync(up => up.Rank);
            userPoint.Rank = lastRank + 1;

            // Add new user to database and save
            await _unitOfWork.GetRepository<UserPoint>().InsertAsync(userPoint);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetUserPointModel>> GetAllUserPointsAsync(int index, int pageSize, string? idSearch, string? userIdSearch)
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

            // Get list of user point
            IQueryable<UserPoint> query = _unitOfWork.GetRepository<UserPoint>().Entities;

            // Check if user want to search user point by Id
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                // Convert to guid
                Guid.TryParse(idSearch, out Guid id);

                // Search user by id
                query = query.Where(up => up.Id.Equals(id));
            }

            // Check if user want to search a user point by userId
            if (!string.IsNullOrWhiteSpace(userIdSearch))
            {
                // Convert to guid
                Guid.TryParse(userIdSearch, out Guid id);

                // Search user by id
                query = query.Where(up => up.UserId.Equals(id));
            }

            // Skip deleted item
            query = query.Where(up => !up.DeletedTime.HasValue);

            // Sort the list by Rank
            query = query.OrderBy(up => up.Rank);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<UserPoint> resultQuery = await _unitOfWork.GetRepository<UserPoint>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetUserPointModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map ApplicationUser to ModelView in order to filter unnecessary data 
                GetUserPointModel userPointModel = _mapper.Map<GetUserPointModel>(item);

                // Get user's name base on userId
                userPointModel.UserName = _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                    .Where(u => u.Id.Equals(item.UserId))
                                                    .Select(u => u.Name)
                                                    .FirstOrDefault()
                                                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "User's name not found!");

                return userPointModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetUserPointModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        public async Task<GetUserPointModel> GetCurrentUserPointdAsync()
        {
            // Get current login user id
            string? userId = _tokenService.GetCurrentUserId();

            // Get user point by user Id
            UserPoint? userPoint = await _unitOfWork.GetRepository<UserPoint>().Entities
                                            .Where(up => up.UserId.Equals(Guid.Parse(userId)))
                                            .FirstOrDefaultAsync();

            // Validate if user point is existed
            if (userPoint == null || userPoint.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "User point not found!");

            // Map ApplicationUser to ModelView in order to filter unnecessary data 
            GetUserPointModel userPointModel = _mapper.Map<GetUserPointModel>(userPoint);

            // Get user's name base on userId
            userPointModel.UserName = _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                .Where(u => u.Id.Equals(userPoint.UserId))
                                                .Select(u => u.Name)
                                                .FirstOrDefault()
                                                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "User's name not found!");

            return userPointModel;
        }

        public async Task UpdateUserPointAsync(PutUserPointModel updatedUserPoint)
        {
            // Get current login user id
            string? userId = _tokenService.GetCurrentUserId();

            // Validate if user existed
            ApplicationUser? existingUser = await _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                            .Where(u => u.Id.Equals(Guid.Parse(userId)))
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This user is not exist!");

            // Validate if user point existed
            UserPoint? existingUserPoint = await _unitOfWork.GetRepository<UserPoint>().Entities
                                                            .Where(u => u.UserId.Equals(Guid.Parse(userId)))
                                                            .FirstOrDefaultAsync();

            // Create user point if user doesn't have point
            if (existingUserPoint == null)
            {
                PostUserPointModel newUserPoint = new PostUserPointModel()
                {
                    UserId = userId,
                };

                // Initialize a new user point
                await CreateUserPointAsync(newUserPoint);

                // Assigned new user point
                existingUserPoint = await _unitOfWork.GetRepository<UserPoint>().Entities
                                                            .Where(u => u.UserId.Equals(Guid.Parse(userId)))
                                                            .FirstOrDefaultAsync();
            }

            existingUserPoint.Point += updatedUserPoint.AdditionalPoint;

            // Get the user with the next higher rank
            UserPoint? nextHigherRankUser = await _unitOfWork.GetRepository<UserPoint>().Entities
                .Where(up => up.Rank == existingUserPoint.Rank - 1)
                .FirstOrDefaultAsync();
            
            // If next higher rank user exist and the current user point is bigger than the next rank user
            if (nextHigherRankUser != null && existingUserPoint.Point > nextHigherRankUser.Point)
            {
                // Swap ranks
                int? tempRank = existingUserPoint.Rank;
                existingUserPoint.Rank = nextHigherRankUser.Rank;
                nextHigherRankUser.Rank = tempRank;

                // Update audit field
                existingUser.LastUpdatedTime = CoreHelper.SystemTimeNow;
                nextHigherRankUser.LastUpdatedTime = CoreHelper.SystemTimeNow;

                // Update the database
                await _unitOfWork.GetRepository<UserPoint>().UpdateAsync(existingUserPoint);
                await _unitOfWork.GetRepository<UserPoint>().UpdateAsync(nextHigherRankUser);
            }
            else
            {
                // Update audit field
                existingUser.LastUpdatedTime = CoreHelper.SystemTimeNow;

                // Update the database
                await _unitOfWork.GetRepository<UserPoint>().UpdateAsync(existingUserPoint);
            }

            IList<Badge> badges = await _unitOfWork.GetRepository<Badge>().Entities.ToListAsync();
            IList<UserBadge> userBadges = await _unitOfWork.GetRepository<UserBadge>().Entities
                                                                                .Where(ub => ub.UserId.Equals(Guid.Parse(userId)))    
                                                                                .ToListAsync();
            // Give badge to user
            if (!userBadges.Any()) {
                foreach (Badge badge in badges)
                {
                    // If user has enough point, give badge to user
                    if (existingUserPoint.Point >= badge.conditionalPoint)
                    {
                        PostUserBadgeModel postUserBadgeModel = new PostUserBadgeModel
                        {
                            UserId = userId,
                            BadgeId = badge.Id.ToString(),
                        };

                        await _userBadgeService.CreateUserBadgeAsync(postUserBadgeModel);
                    }
                }
            }
            else
            {
                foreach (Badge badge in badges)
                {
                    // Check if user already contain the badges
                    foreach (UserBadge item in userBadges)
                    {
                        if (item.BadgeId.Equals(badge.Id))
                        {
                            continue;
                        }
                        // If user has enough point, give badge to user
                        else if (existingUserPoint.Point >= badge.conditionalPoint)
                        {
                            PostUserBadgeModel postUserBadgeModel = new PostUserBadgeModel
                            {
                                UserId = userId,
                                BadgeId = badge.Id.ToString(),
                            };

                            await _userBadgeService.CreateUserBadgeAsync(postUserBadgeModel);
                        }
                    }
                }
            }

            await _unitOfWork.SaveAsync();
        }
    }
}

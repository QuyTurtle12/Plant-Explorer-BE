using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.Constants.Enum.EnumBadge;
using Plant_Explorer.Core.ExceptionCustom;
using Plant_Explorer.Core.Utils;

namespace Plant_Explorer.Services.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public const int INACTIVE = 0;
        public const int ACTIVE = 1;

        public BadgeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateBadgeAsync(PostBadgeModel newBadge)
        {
            GeneralValidation(newBadge);

            // Mapping model to entities
            Badge badge = _mapper.Map<Badge>(newBadge);

            // Add new badge to database and save
            await _unitOfWork.GetRepository<Badge>().InsertAsync(badge);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteBadgeAsync(string id)
        {
            // Validate if user existed
            Badge? existingBadge = await _unitOfWork.GetRepository<Badge>().Entities
                                                            .Where(b => b.Id.Equals(Guid.Parse(id)))
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This badge is not exist!");

            // Delete item
            existingBadge.Status = INACTIVE;

            // Update audit fields
            existingBadge.LastUpdatedTime = CoreHelper.SystemTimeNow;
            existingBadge.DeletedTime = existingBadge.LastUpdatedTime;


            // Save
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetBadgeModel>> GetAllBadgesAsync(int index, int pageSize, string? idSearch, string? nameSearch, EnumBadge? badgeType)
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

            // Get list of badge
            IQueryable<Badge> query = _unitOfWork.GetRepository<Badge>().Entities;

            // Check if user want to search a badge by badgeId
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                // Convert to guid
                Guid.TryParse(idSearch, out Guid id);

                // Search user by id
                query = query.Where(b => b.Id.Equals(id));
            }

            // Check if user want to search badges by name 
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                // Search user by name
                query = query.Where(b => b.Name.Contains(nameSearch));
            }

            // Check if user want to get a list of badge base on badge's type
            switch (badgeType)
            {
                // Get list of gold badge
                case EnumBadge.Gold:
                    query = query.Where(b => b.Type!.Equals(EnumBadge.Gold.ToString()));
                    break;
                // Get list of silver badge
                case EnumBadge.Silver:
                    query = query.Where(b => b.Type!.Equals(EnumBadge.Silver.ToString()));
                    break;
                // Get list of copper badge
                case EnumBadge.Copper:
                    query = query.Where(b => b.Type!.Equals(EnumBadge.Copper.ToString()));
                    break;

                default: break;
            }

            // Skip deleted item
            query = query.Where(b => !b.DeletedTime.HasValue);

            // Sort the list by name
            query = query.OrderBy(b => b.Name);

            // Change to paginated list type to facilitate filtering process
            PaginatedList<Badge> resultQuery = await _unitOfWork.GetRepository<Badge>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetBadgeModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map ApplicationUser to ModelView in order to filter unnecessary data 
                GetBadgeModel badgeModel = _mapper.Map<GetBadgeModel>(item);

                // Format audit fields
                badgeModel.CreatedTime = item.CreatedTime?.ToString("dd-MM-yyyy");
                badgeModel.LastUpdatedTime = item.LastUpdatedTime?.ToString("dd-MM-yyyy");

                return badgeModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetBadgeModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        public async Task<GetBadgeModel> GetBadgeByIdAsync(string id)
        {
            Badge? badge = await _unitOfWork.GetRepository<Badge>().GetByIdAsync(Guid.Parse(id));

            // Validate if user is existed
            if (badge == null || badge.DeletedTime.HasValue) throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Badge not found!");

            GetBadgeModel badgeModel = _mapper.Map<GetBadgeModel>(badge);

            // Format audit fields
            badgeModel.CreatedTime = badge.CreatedTime?.ToString("dd-MM-yyyy");
            badgeModel.LastUpdatedTime = badge.LastUpdatedTime?.ToString("dd-MM-yyyy");

            return badgeModel;
        }

        public async Task UpdateBadgeAsync(string id, PutBadgeModel updatedBadge)
        {
            // Validate if user existed
            Badge? existingBadge = await _unitOfWork.GetRepository<Badge>().Entities
                                                            .Where(b => b.Id.Equals(Guid.Parse(id)))
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "This badge is not exist!");
            // Validate input
            GeneralValidation(updatedBadge);

            // Mapping model to entities
            _mapper.Map(updatedBadge, existingBadge);

            // Update audit field
            existingBadge.LastUpdatedTime = CoreHelper.SystemTimeNow;

            // Update badge info and save
            _unitOfWork.GetRepository<Badge>().Update(existingBadge);
            await _unitOfWork.SaveAsync();
        }

        private void GeneralValidation(BaseBadgeModel badge)
        {
            // Validate badge's name
            if (string.IsNullOrWhiteSpace(badge.Name)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");

            // Validate badge's type
            if (string.IsNullOrWhiteSpace(badge.Type)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Type must not be empty!");

            // Validate badge's type format
            if (badge.Type != EnumBadge.Gold.ToString() &&
                badge.Type != EnumBadge.Silver.ToString() &&
                badge.Type != EnumBadge.Copper.ToString())
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Type must be Copper, Silver, or Gold!");
            }


        }
    }
}

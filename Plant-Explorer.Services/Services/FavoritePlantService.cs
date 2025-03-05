using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.FavoritePlantModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;

namespace Plant_Explorer.Services.Services
{
    public class FavoritePlantService : IFavoritePlantService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public FavoritePlantService(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task CreateUserFavoritePlantAsync(PostFavoritePlantModel newFavoritePlant)
        {
            // Get current login user id
            string? userId = _tokenService.GetCurrentUserId();

            await GeneralValidationAsync(userId, newFavoritePlant);

            // Mapping model to entities
            FavoritePlant favoritePlant = _mapper.Map<FavoritePlant>(newFavoritePlant);
            favoritePlant.UserId = Guid.Parse(userId);

            // Add new badge to database and save
            await _unitOfWork.GetRepository<FavoritePlant>().InsertAsync(favoritePlant);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteUserFavoritePlantAsync(string id)
        {
            // Check id format
            Guid idGuid;
            if (!Guid.TryParse(id, out idGuid))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Invalid User ID format.");
            }

            // Get favorite plant from FavoritePlant's id
            FavoritePlant? favoritePlant = await _unitOfWork.GetRepository<FavoritePlant>().GetByIdAsync(idGuid) ??
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This favorite plant id is not existed");

            // Save to database
            await _unitOfWork.GetRepository<FavoritePlant>().DeleteAsync(favoritePlant);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetFavoritePlantModel>> GetUserFavoritePlantsAsync(int index, int pageSize)
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

            // Get current login user id
            string? userId = _tokenService.GetCurrentUserId();

            // user Id checking
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "User Id can not be empty!");
            }

            // Get list of user favorite plant
            IQueryable<FavoritePlant> query = _unitOfWork.GetRepository<FavoritePlant>().Entities
                                                    .Where(ub => ub.UserId.Equals(Guid.Parse(userId)));

            // Change to paginated list type to facilitate filtering process
            PaginatedList<FavoritePlant> resultQuery = await _unitOfWork.GetRepository<FavoritePlant>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetFavoritePlantModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map ApplicationUser to ModelView in order to filter unnecessary data 
                GetFavoritePlantModel plantModel = _mapper.Map<GetFavoritePlantModel>(item);

                // Get user name
                plantModel.UserName = _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                    .Where(u => u.Id.Equals(item.UserId))
                                                    .Select(u => u.Name)
                                                    .FirstOrDefault()
                                                    ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This user's name is null");

                // Get platn name
                plantModel.PlantName = _unitOfWork.GetRepository<Plant>().Entities
                                    .Where(p => p.Id.Equals(item.PlantId))
                                    .Select(p => p.Name)
                                    .FirstOrDefault()
                                    ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This plant's name is null");

                return plantModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetFavoritePlantModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        private async Task GeneralValidationAsync(string userId, PostFavoritePlantModel userFavoritePlant)
        {

            // Validate plant
            if (string.IsNullOrWhiteSpace(userFavoritePlant.PlantId)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Plant must not be empty!");

            // Check plant id format
            Guid plantIdGuid;
            if (!Guid.TryParse(userFavoritePlant.PlantId, out plantIdGuid))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Invalid Plant ID format.");
            }


            // Validate if plant existed
            Plant? existingPlant = await _unitOfWork.GetRepository<Plant>().Entities
                                                            .Where(b => b.Id.Equals(Guid.Parse(userFavoritePlant.PlantId)) && !b.DeletedTime.HasValue)
                                                            .FirstOrDefaultAsync()
                                                            ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This plant is not exist!");

            // Validate if user has already marked the plant as favorite plant
            FavoritePlant? existingUserFavoritePlant = await _unitOfWork.GetRepository<FavoritePlant>().Entities
                                                            .Where(fp => fp.UserId.Equals(Guid.Parse(userId)) && fp.PlantId.Equals(Guid.Parse(userFavoritePlant.PlantId)))
                                                            .FirstOrDefaultAsync();

            if (existingUserFavoritePlant != null) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "This plant has already marked favorite by this user!");
        }
    }
}

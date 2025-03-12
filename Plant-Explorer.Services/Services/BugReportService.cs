using AutoMapper;
using Microsoft.AspNetCore.Http;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel;
using Plant_Explorer.Contract.Repositories.ModelViews.BugReportModel;
using Plant_Explorer.Contract.Repositories.ModelViews.UserModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;

namespace Plant_Explorer.Services.Services
{
    public class BugReportService : IBugReportService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BugReportService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateUserReportAsync(PostBugReportModel newBugReport)
        {
            GeneralValidation(newBugReport);

            // Mapping model to entities
            BugReport bugReport = _mapper.Map<BugReport>(newBugReport);

            bugReport.CreatedBy = newBugReport.userId;

            // Add new badge to database and save
            await _unitOfWork.GetRepository<BugReport>().InsertAsync(bugReport);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<GetBugReportModel>> GetAllUserReportsAsync(int index, int pageSize, string? idSearch, string? nameSearch, string? userIdSearch)
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

            // Get list of bug report
            IQueryable<BugReport> query = _unitOfWork.GetRepository<BugReport>().Entities;

            // Check if user want to search bug reports by bug report id
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                // Convert to guid
                Guid.TryParse(idSearch, out Guid id);

                // Search user by id
                query = query.Where(br => br.Id.Equals(id));
            }

            // Check if user want to search bug report by name 
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                // Search user by name
                query = query.Where(br => br.Name.Contains(nameSearch));
            }

            // Check if user want to search bug reports by user id
            if (!string.IsNullOrWhiteSpace(userIdSearch))
            {
                // Convert to guid
                Guid.TryParse(userIdSearch, out Guid id);

                // Search user by id
                query = query.Where(br => br.UserId.Equals(id));
            }

            // Skip deleted item
            query = query.Where(br => !br.DeletedTime.HasValue);

            // Sort the list by created time
            query = query.OrderBy(br => br.CreatedTime);


            // Change to paginated list type to facilitate filtering process
            PaginatedList<BugReport> resultQuery = await _unitOfWork.GetRepository<BugReport>().GetPagging(query, index, pageSize);

            // Filter unnecessary data
            IReadOnlyCollection<GetBugReportModel> responseItems = resultQuery.Items.Select(item =>
            {
                // Map BugReport to ModelView in order to filter unnecessary data 
                GetBugReportModel bugReportModel = _mapper.Map<GetBugReportModel>(item);

                bugReportModel.CreatedTime = item.CreatedTime?.ToString("dd-MM-yyyy");

                // Get reported user's email
                bugReportModel.CreatedBy = _unitOfWork.GetRepository<ApplicationUser>().Entities
                                                    .Where(u => u.Id.Equals(item.UserId))
                                                    .Select(u => u.Email)
                                                    .FirstOrDefault()
                                                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "User's Email not found!");

                return bugReportModel;
            }).ToList();

            // Create a new paginated list for response data
            PaginatedList<GetBugReportModel> paginatedList = new(
                responseItems,
                resultQuery.TotalCount,
                resultQuery.PageNumber,
                resultQuery.TotalPages
                );

            return paginatedList;
        }

        private void GeneralValidation(BaseBugReportModel bugReport)
        {
            // Validate badge's name (title)
            if (string.IsNullOrWhiteSpace(bugReport.Name)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Name must not be empty!");

            // Validate badge's context
            if (string.IsNullOrWhiteSpace(bugReport.Context)) throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Context must not be empty!");
        }
    }
}

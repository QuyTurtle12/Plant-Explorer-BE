using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.BugReportModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BugReportsController : ControllerBase
    {
        private readonly IBugReportService _bugReportService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bugReportService"></param>
        public BugReportsController(IBugReportService bugReportService)
        {
            _bugReportService = bugReportService;
        }

        /// <summary>
        /// Get all bug report
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch">bug report id</param>
        /// <param name="nameSearch">bug report title/name</param>
        /// <param name="userIdSearch">reported user id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/bug-reports")]
        public async Task<IActionResult> GetAllBugReports(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, string? userIdSearch = null)
        {
            PaginatedList<GetBugReportModel> result = await _bugReportService.GetAllUserReportsAsync(index, pageSize, idSearch, nameSearch, userIdSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetBugReportModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Create a new bug report
        /// </summary>
        /// <param name="newBugReport"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/bug-reports")]
        public async Task<IActionResult> CreateUserReport(PostBugReportModel newBugReport)
        {
            await _bugReportService.CreateUserReportAsync(newBugReport);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Create a new bug report successfully"
                ));
        }
    }
}

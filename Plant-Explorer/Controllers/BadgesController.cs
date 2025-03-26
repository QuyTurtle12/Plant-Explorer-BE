using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel;
using Plant_Explorer.Contract.Repositories.ModelViews.UserModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.Constants.Enum.EnumBadge;
using Plant_Explorer.Services.Services;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="badgeService"></param>
        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        /// <summary>
        /// Get all badge
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch">badge id</param>
        /// <param name="nameSearch">badge name</param>
        /// <param name="badgeType">1 = Gold, 2 = Silver, 3 = Copper</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBadges(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, EnumBadge? badgeType = null)
        {
            PaginatedList<GetBadgeModel> result = await _badgeService.GetAllBadgesAsync(index, pageSize, idSearch, nameSearch, badgeType);
            return Ok(new BaseResponseModel<PaginatedList<GetBadgeModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Get badge
        /// </summary>
        /// <param name="id">badge id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBadge(string id)
        {
            GetBadgeModel result = await _badgeService.GetBadgeByIdAsync(id);
            return Ok(new BaseResponseModel<GetBadgeModel>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }


        /// <summary>
        /// Create a new badge
        /// </summary>
        /// <param name="newBadge">Type: Gold, Silver, Copper</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateBadge(PostBadgeModel newBadge)
        {
            await _badgeService.CreateBadgeAsync(newBadge);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Create a new badge successfully"
                ));
        }

        /// <summary>
        /// Update a badge
        /// </summary>
        /// <param name="id">badge id</param>
        /// <param name="updatedBage">Type: Gold, Silver, Copper</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateBadge(string id, PutBadgeModel updatedBage)
        {
            await _badgeService.UpdateBadgeAsync(id, updatedBage);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Update a badge successfully"
                ));
        }

        /// <summary>
        /// Delete badge
        /// </summary>
        /// <param name="id">badge id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBadge(string id)
        {
            await _badgeService.DeleteBadgeAsync(id);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Delete a badge successfully"
                ));
        }
    }
}

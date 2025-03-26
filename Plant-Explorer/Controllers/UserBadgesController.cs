using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel;
using System.ComponentModel.DataAnnotations;
using Plant_Explorer.Contract.Repositories.ModelViews.BadgeModel;
using Plant_Explorer.Services.Services;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserBadgesController : ControllerBase
    {
        private readonly IUserBadgeService _userBadgeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userBadgeService"></param>
        public UserBadgesController(IUserBadgeService userBadgeService)
        {
            _userBadgeService = userBadgeService;
        }

        /// <summary>
        /// Get all user badges of a user
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/user-badges")]
        public async Task<IActionResult> GetCurrentUserBadges(int index = 1, int pageSize = 10) 
        {
            PaginatedList<GetUserBadgeModel> result = await _userBadgeService.GetUserBadgesAsync(index, pageSize);
            return Ok(new BaseResponseModel<PaginatedList<GetUserBadgeModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        //[HttpPost]
        //[Route("badge")]
        //public async Task<IActionResult> CreateUserBadge(PostUserBadgeModel newUserBadge)
        //{
        //    await _userBadgeService.CreateUserBadgeAsync(newUserBadge);
        //    return Ok(new BaseResponseModel(
        //        statusCode: StatusCodes.Status201Created,
        //        code: ResponseCodeConstants.SUCCESS,
        //        message: "Create a new user badge successfully"
        //        ));
        //}
    }
}

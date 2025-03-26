using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.UserPointModel;
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
    public class UserPointsController : ControllerBase
    {
        private readonly IUserPointService _userPointService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPointService"></param>
        public UserPointsController(IUserPointService userPointService)
        {
            _userPointService = userPointService;
        }

        /// <summary>
        /// Get all user point/rank
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch">User point id</param>
        /// <param name="userIdSearch">User id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/user-points")]
        public async Task<IActionResult> GetAllUserPoint(int index = 1, int pageSize = 10, string? idSearch = null, string? userIdSearch = null)
        {
            PaginatedList<GetUserPointModel> result = await _userPointService.GetAllUserPointsAsync(index, pageSize, idSearch, userIdSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetUserPointModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Get curent logged in user point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/user-points/current-point")]
        public async Task<IActionResult> GetCurrentUserPoint()
        {
            GetUserPointModel result = await _userPointService.GetCurrentUserPointdAsync();
            return Ok(new BaseResponseModel<GetUserPointModel>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Create a new user point
        /// </summary>
        /// <param name="newUserPoint"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("user")]
        //public async Task<IActionResult> CreateUserPoint(PostUserPointModel newUserPoint)
        //{
        //    await _userPointService.CreateUserPointAsync(newUserPoint);
        //    return Ok(new BaseResponseModel(
        //        statusCode: StatusCodes.Status201Created,
        //        code: ResponseCodeConstants.SUCCESS,
        //        message: "Create a user point successfully"
        //        ));
        //}


        //[HttpPut]
        //[Route("/api/user-points")]
        //public async Task<IActionResult> UpdateUserPoint(PutUserPointModel updatedUserPoint)
        //{
        //    await _userPointService.UpdateUserPointAsync(updatedUserPoint);
        //    return Ok(new BaseResponseModel(
        //        statusCode: StatusCodes.Status200OK,
        //        code: ResponseCodeConstants.SUCCESS,
        //        message: "Update a user point successfully"
        //        ));
        //}
    }
}

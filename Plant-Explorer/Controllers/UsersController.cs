using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.AuthModel;
using Plant_Explorer.Contract.Repositories.ModelViews.UserModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.Constants.Enum.EnumUser;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all the user in database
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch"></param>
        /// <param name="nameSearch"></param>
        /// <param name="emailSearch"></param>
        /// <param name="role"> 1 = Children, 2 = Staff, 3 = Admin</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, string? emailSearch = null, EnumRole? role = null)
        {
            PaginatedList<GetUserModel> result = await _userService.GetAllUsersAsync(index, pageSize, idSearch, nameSearch, emailSearch, role);
            return Ok(new BaseResponseModel<PaginatedList<GetUserModel>>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 data: result,
                 additionalData: null,
                 message: "Finished"
                ));
        }

        /// <summary>
        /// Get current logged in user profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current-user")]
        public async Task<IActionResult> GetUserProfile()
        {
            GetUserModel result = await _userService.GetUserProfileAsync();
            return Ok(new BaseResponseModel<GetUserModel>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Create a new child
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("children")]
        //public async Task<IActionResult> CreateChildren(PostUserModel newUser)
        //{
        //    newUser.RoleName = "Children";
        //    await _userService.CreateUserAsync(newUser);
        //    return Ok(new BaseResponseModel(
        //        statusCode: StatusCodes.Status201Created,
        //        code: ResponseCodeConstants.SUCCESS,
        //        message: "Create a new child successfully"
        //        ));
        //}

        /// <summary>
        /// Create a new staff
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("staff")]
        public async Task<IActionResult> CreateStaff(RegisterRequest newUser)
        {
            await _userService.CreateUserAsync(newUser);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Create a new staff successfully"
                ));
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="updatedUser">new info</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser(string id, PutUserModel updatedUser)
        {
            await _userService.UpdateUserAsync(id, updatedUser);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Update a user successfully"
                ));
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Delete a user successfully"
                ));
        }
    }
}

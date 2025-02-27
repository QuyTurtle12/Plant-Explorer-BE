using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
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
        /// <param name="role"> 1 = Children, 2 = Staff, 3 = Admin</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int index = 1, int pageSize = 10, string? idSearch = null, string? nameSearch = null, EnumRole? role = null)
        {
            PaginatedList<GetUserModel> result = await _userService.GetAllUsersAsync(index, pageSize, idSearch, nameSearch, role);
            return Ok(new BaseResponseModel<PaginatedList<GetUserModel>>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 data: result,
                 additionalData: null,
                 message: "Finished"
                ));
        }
    }
}

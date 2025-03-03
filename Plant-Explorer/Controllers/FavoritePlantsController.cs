using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.ModelViews.FavoritePlantModel;
using Plant_Explorer.Contract.Repositories.ModelViews.UserBadgeModel;
using Plant_Explorer.Contract.Repositories.PaggingItems;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Services.Services;

namespace Plant_Explorer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritePlantsController : ControllerBase
    {
        private readonly IFavoritePlantService _favoritePlantsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="favoritePlantsService"></param>
        public FavoritePlantsController(IFavoritePlantService favoritePlantsService)
        {
            _favoritePlantsService = favoritePlantsService;
        }

        /// <summary>
        /// Get all favorite plants of a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUserFavoritePlants([Required] string userId, int index = 1, int pageSize = 10)
        {
            PaginatedList<GetFavoritePlantModel> result = await _favoritePlantsService.GetUserFavoritePlantsAsync(index, pageSize, userId);
            return Ok(new BaseResponseModel<PaginatedList<GetFavoritePlantModel>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                additionalData: null,
                message: "Finished"
                ));
        }

        /// <summary>
        /// Mark as user's favorite plant
        /// </summary>
        /// <param name="newUserFavoritePlant"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("plant")]
        public async Task<IActionResult> CreateUserFavoritePlant(PostFavoritePlantModel newUserFavoritePlant)
        {
            await _favoritePlantsService.CreateUserFavoritePlantAsync(newUserFavoritePlant);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status201Created,
                code: ResponseCodeConstants.SUCCESS,
                message: "Mark this plant as favorite successfully"
                ));
        }

        /// <summary>
        /// Remove favorite plant mark
        /// </summary>
        /// <param name="id">FavoritePlant id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteUserFavoritePlant([Required] string id)
        {
            await _favoritePlantsService.DeleteUserFavoritePlantAsync(id);
            return Ok(new BaseResponseModel(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Unmark this favorite plant successfully"
                ));
        }
    }
}

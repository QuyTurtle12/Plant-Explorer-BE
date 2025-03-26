using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews.AvatarModel;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarsController : ControllerBase
    {
        private readonly IAvatarService _avatarService;

        public AvatarsController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        /// <summary>
        /// Get all avatars
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var avatars = await _avatarService.GetAllAvatarsAsync();
            return Ok(avatars);
        }

        /// <summary>
        /// Get avatar by avatar id
        /// </summary>
        /// <param name="id">Avatar Id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var avatar = await _avatarService.GetAvatarByIdAsync(id);
            return Ok(avatar);
        }

        /// <summary>
        /// Create avatar
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAvatarRequest request)
        {
            var avatar = await _avatarService.CreateAvatarAsync(request);
            return Ok(avatar);
        }

        /// <summary>
        /// Update avatar info
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAvatarRequest request)
        {
            var avatar = await _avatarService.UpdateAvatarAsync(request);
            return Ok(avatar);
        }

        /// <summary>
        /// Update user avatar
        /// </summary>
        /// <param name="id">Avatar Id</param>
        /// <returns></returns>
        [HttpPut]
        [Route("user/{id}")]
        public async Task<IActionResult> UpdateUserAvatar(Guid id)
        {
            await _avatarService.UpdateUserAvatarAsync(id);
            return Ok();
        }

        /// <summary>
        /// Delete avatar
        /// </summary>
        /// <param name="id">Avatar Id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _avatarService.DeleteAvatarAsync(id);
            return Ok(new { message = "Avatar deleted successfully" });
        }
    }

}

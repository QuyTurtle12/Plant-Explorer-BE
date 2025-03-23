using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews.AvatarModel;
using Plant_Explorer.Contract.Services.Interface;

namespace Plant_Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : ControllerBase
    {
        private readonly IAvatarService _avatarService;

        public AvatarController(IAvatarService avatarService)
        {
            _avatarService = avatarService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var avatars = await _avatarService.GetAllAvatarsAsync();
            return Ok(avatars);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var avatar = await _avatarService.GetAvatarByIdAsync(id);
            return Ok(avatar);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAvatarRequest request)
        {
            var avatar = await _avatarService.CreateAvatarAsync(request);
            return Ok(avatar);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAvatarRequest request)
        {
            var avatar = await _avatarService.UpdateAvatarAsync(request);
            return Ok(avatar);
        }

        [HttpPut]
        [Route("user")]
        public async Task<IActionResult> UpdateUserAvatar(Guid avatarId)
        {
            await _avatarService.UpdateUserAvatarAsync(avatarId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _avatarService.DeleteAvatarAsync(id);
            return Ok(new { message = "Avatar deleted successfully" });
        }
    }

}

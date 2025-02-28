using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plant_Explorer.Contract.Repositories.ModelViews.AuthModel;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Services.Infrastructure;

namespace Plant_Explorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var response = await _authService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Log exception details as needed
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }

        }
    }
}
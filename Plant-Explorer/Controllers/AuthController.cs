using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // You could differentiate exception types for better error handling if needed
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new { Message = "This is a public endpoint accessible to everyone." });
        }

        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult Authenticated()
        {
            foreach (var claim in HttpContext.User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            return Ok(new { Message = "You are authenticated!" });
        }

        [Authorize(Roles = "Children")]
        [HttpGet("children")]
        public IActionResult ChildrenOnly()
        {
            bool isInRole = HttpContext.User.IsInRole("Children");
            Console.WriteLine($"IsInRole(\"Children\"): {isInRole}");

            return Ok(new { Message = "Hello, Child user!" });
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("staff")]
        public IActionResult StaffOnly()
        {
            return Ok(new { Message = "Hello, Staff member!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new { Message = "Hello, Admin!" });
        }

    }
}
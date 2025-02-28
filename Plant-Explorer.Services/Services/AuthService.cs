using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.AuthModel;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Services.Infrastructure;

namespace Plant_Explorer.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Contract.Repositories.Base.JwtSettings _jwtSettings;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<Contract.Repositories.Base.JwtSettings> jwtOptions,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _jwtSettings = jwtOptions.Value;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            // Find user by email (or username, depending on your requirements)
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Verify the provided password against the hashed password stored in the database
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginRequest.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Create JWT token using the helper method (from your provided Authentication class)
            string token = Authentication.CreateToken(user.Id.ToString(), _jwtSettings, isRefresh: false);

            return new LoginResponse
            {
                Token = token,
                UserId = user.Id.ToString()
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                throw new Exception("Passwords do not match.");
            }

            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                throw new Exception("Email is already registered.");
            }

            var newUser = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                Name = registerRequest.Name,
            };

            // Create the user with the hashed password
            var result = await _userManager.CreateAsync(newUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                // Combine errors into a single string for simplicity
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            return new RegisterResponse
            {
                Message = "Registration successful.",
                UserId = newUser.Id.ToString()
            };
        }


    }
}

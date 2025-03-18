using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Contract.Repositories.Entity;
using Plant_Explorer.Contract.Repositories.ModelViews.AuthModel;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Services.Infrastructure;

namespace Plant_Explorer.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<JwtSettings> jwtOptions,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtOptions.Value;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            // Find user by email (or username, depending on your requirements)
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            var roles = await _userManager.GetRolesAsync(user);
            string userRole = roles.FirstOrDefault() ?? "Not Found Role";
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
            string token = Authentication.CreateToken(user.Id.ToString(), user.UserName, userRole, _jwtSettings, isRefresh: false);

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

            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                throw new Exception("Email is already registered.");
            }

            // Look up the default role 
            var defaultRole = await _roleManager.FindByNameAsync("Children");
            if (defaultRole == null)
            {
                throw new Exception("Default role not found. Please contact support.");
            }

            // Create a new ApplicationUser with a valid RoleId.
            var newUser = new ApplicationUser
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                Name = registerRequest.Name,
                RoleId = defaultRole.Id,
                Age = registerRequest.Age
            };

            var result = await _userManager.CreateAsync(newUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            // Optionally, assign the role to the user via Identity.
            await _userManager.AddToRoleAsync(newUser, defaultRole.Name);

            return new RegisterResponse
            {
                Message = "Registration successful.",
                UserId = newUser.Id.ToString()
            };
        }

        public async Task<GoogleLoginResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                throw new Exception("IdToken is required.");
            }

            try
            {
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
                string firebaseUid = decodedToken.Uid;
                string email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
                // Default Role
                string defaultRoleName = "Children"; 

                // Find an existing user by email. If not found, create a new one.
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Create a new user for Google sign in
                    var defaultRole = await _roleManager.FindByNameAsync(defaultRoleName);
                    if (defaultRole == null)
                    {
                        throw new Exception("Default role not found. Please contact support.");
                    }

                    user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        Name = email,
                        RoleId = defaultRole.Id
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                        throw new Exception(errors);
                    }

                    await _userManager.AddToRoleAsync(user, defaultRole.Name);
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);
                string userRole = roles.FirstOrDefault() ?? defaultRoleName;

                // Create JWT token for system.
                string token = Authentication.CreateToken(user.Id.ToString(), user.UserName, userRole, _jwtSettings, isRefresh: false);

                return new GoogleLoginResponse
                {
                    Token = token,
                    UserId = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                throw new UnauthorizedException("Invalid Google token: " + ex.Message);
            }
        }


    }
}

using Plant_Explorer.Contract.Repositories.ModelViews.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plant_Explorer.Contract.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<GoogleLoginResponse> GoogleLoginAsync(GoogleLoginRequest request);
    }
}

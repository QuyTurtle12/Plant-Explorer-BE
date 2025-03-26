using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Plant_Explorer.Contract.Repositories.Base;
using Plant_Explorer.Core.Constants;
using Plant_Explorer.Core.ExceptionCustom;

namespace Plant_Explorer.Services.Infrastructure
{
    public class Authentication
    {
        public static string CreateToken(string id, string username, string role, JwtSettings jwtSettings, bool isRefresh = false)
        {
            // Add claims for id, username, and role.
            List<Claim> claims = new List<Claim>
            {
                new Claim("id", id),
                new Claim("username", username),
                new Claim("role", role)
            };

            DateTime expiration = DateTime.Now.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);
            if (isRefresh)
            {
                expiration = DateTime.Now.AddDays(jwtSettings.RefreshTokenExpirationDays);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            // Optionally override the header algorithm to be exactly "HS512"
            //token.Header["alg"] = "HS512";

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetUserIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                if (httpContextAccessor.HttpContext == null || !httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    throw new UnauthorizedException("Need Authorization");
                }

                string? authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];

                if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedException($"Invalid authorization header: {authorizationHeader}");
                }

                string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(jwtToken))
                {
                    throw new UnauthorizedException("Invalid token format");
                }

                var token = tokenHandler.ReadJwtToken(jwtToken);
                var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "id");

                return idClaim?.Value ?? throw new UnauthorizedException("Cannot get userId from token");
            }
            catch (UnauthorizedException ex)
            {
                var errorResponse = new
                {
                    data = "An unexpected error occurred.",
                    message = ex.Message,
                    statusCode = StatusCodes.Status401Unauthorized,
                    code = "Unauthorized!"
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);

                if (httpContextAccessor.HttpContext != null)
                {
                    httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    httpContextAccessor.HttpContext.Response.ContentType = "application/json";
                    httpContextAccessor.HttpContext.Response.WriteAsync(jsonResponse).Wait();
                }

                httpContextAccessor.HttpContext?.Response.WriteAsync(jsonResponse).Wait();

                throw; // Re-throw the exception to maintain the error flow
            }
        }
        public static string GetUserIdFromHttpContext(HttpContext httpContext)
        {
            try
            {
                if (!httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    throw new UnauthorizedException("Need Authorization");
                }

                string? authorizationHeader = httpContext.Request.Headers["Authorization"];

                if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedException($"Invalid authorization header: {authorizationHeader}");
                }

                string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(jwtToken))
                {
                    throw new UnauthorizedException("Invalid token format");
                }

                var token = tokenHandler.ReadJwtToken(jwtToken);
                var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "id");

                return idClaim?.Value ?? throw new UnauthorizedException("Cannot get userId from token");
            }
            catch (UnauthorizedException ex)
            {
                var errorResponse = new
                {
                    data = "An unexpected error occurred.",
                    message = ex.Message,
                    statusCode = StatusCodes.Status401Unauthorized,
                    code = "Unauthorized!"
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.WriteAsync(jsonResponse).Wait();

                throw; // Re-throw the exception to maintain the error flow
            }
        }


        public static string GetUserRoleFromHttpContext(HttpContext httpContext)
        {
            if (httpContext.User?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedException("Need Authorization");

            // Use ClaimTypes.Role to match the mapped role claim.
            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value ?? throw new UnauthorizedException("Cannot get role from token");
        }

        //public static string GetUserRoleFromHttpContext(HttpContext httpContext)
        //{
        //    if (!httpContext.Request.Headers.ContainsKey("Authorization"))
        //        throw new UnauthorizedException("Need Authorization");

        //    string authorizationHeader = httpContext.Request.Headers["Authorization"];

        //    if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //        throw new UnauthorizedException($"Invalid authorization header: {authorizationHeader}");

        //    string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    if (!tokenHandler.CanReadToken(jwtToken))
        //        throw new UnauthorizedException("Invalid token format");

        //    var token = tokenHandler.ReadJwtToken(jwtToken);
        //    var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "role");

        //    if (roleClaim == null)
        //        throw new UnauthorizedException("Cannot get role from token");

        //    return roleClaim.Value;
        //}

        public static async Task HandleForbiddenRequest(HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                return;
            }
            context.Response.Clear();

            int code = (int)HttpStatusCode.Forbidden;
            var error = new ErrorException(code, ResponseCodeConstants.FORBIDDEN, "You don't have permission to access this feature");
            string result = JsonSerializer.Serialize(error);

            context.Response.ContentType = "application/json";
            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            context.Response.StatusCode = code;

            await context.Response.WriteAsync(result);
        }

    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}

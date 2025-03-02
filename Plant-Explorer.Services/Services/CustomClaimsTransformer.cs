using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

public class CustomClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is ClaimsIdentity identity)
        {
            // Check if the standard role claim is missing but a "role" claim exists.
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                var customRole = identity.FindFirst("role");
                if (customRole != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, customRole.Value));
                    Console.WriteLine($"Added standard role claim: {customRole.Value}");
                }
            }
        }
        return Task.FromResult(principal);
    }
}

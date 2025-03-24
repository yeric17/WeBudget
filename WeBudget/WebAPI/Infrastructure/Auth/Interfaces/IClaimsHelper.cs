
using System.Security.Claims;


namespace WebAPI.Infrastructure
{
    public interface IClaimsHelper
    {
        ClaimsPrincipal ClaimsPrincipal { get; }

        List<string>? GetRoles();
        string? GetUserId();
    }
}

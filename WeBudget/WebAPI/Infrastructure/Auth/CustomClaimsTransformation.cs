
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAPI.Domain.Users;


namespace WebAPI.Infrastructure
{
    public class CustomClaimsTransformation : IClaimsTransformation
    {
        private readonly UserManager<User> _usersManager;



        public CustomClaimsTransformation(UserManager<User> userManager)
        {
            _usersManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;

            if (identity is null)
            {
                return principal;
            }

            var userEmail = _usersManager.GetUserName(principal);

            if (userEmail is null)
            {
                return principal;
            }

            var user = await _usersManager.FindByEmailAsync(userEmail);

            if (user is null)
            {
                return principal;
            }


            if (!principal.HasClaim(principal => principal.Type == "userId"))
            {
                identity.AddClaim(new Claim("userId", user.Id));
            }

            var rolesResult = await _usersManager.GetRolesAsync(user);

            if (rolesResult is null)
            {
                return principal;
            }

            var roles = rolesResult.ToList();

            if (!principal.HasClaim(principal => principal.Type == "role") && roles is not null)
            {
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim("role", role));
                }
            }



            return principal;
        }
    }
}

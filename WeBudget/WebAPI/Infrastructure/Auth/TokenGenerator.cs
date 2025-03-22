using Domain.Users;
using Domain.Users.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI.Domain.Users;
using WebAPI.Infrastructure.Auth;

namespace Infrastructure.Auth
{
    internal class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly AuthenticationSettings _authSettings;
        private readonly UserManager<User> _userManager;
        

        public TokenGenerator(IConfiguration configuration, UserManager<User> userManager, IOptions<AuthenticationSettings> authSettings)
        {
            _configuration = configuration;
            _userManager = userManager;
            _authSettings = authSettings.Value;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtKey = _authSettings.JWT.Key;
            var issuer = _authSettings.JWT.Issuer;
            var audience = _authSettings.JWT.Audience;

            if(jwtKey is null || issuer is null || audience is null)
            {
                throw new ArgumentNullException("JWT settings are not configured properly");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}

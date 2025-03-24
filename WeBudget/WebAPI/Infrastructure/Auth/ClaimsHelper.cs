
using System.Security.Claims;

namespace WebAPI.Infrastructure
{
    public class ClaimsHelper : IClaimsHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public ClaimsHelper(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }
        public string? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }

        public List<string>? GetRoles()
        {
            var roles = _httpContextAccessor.HttpContext?.User.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();
            return roles;
        }

        public ClaimsPrincipal ClaimsPrincipal => _httpContextAccessor?.HttpContext?.User ?? new ClaimsPrincipal();
    }
}

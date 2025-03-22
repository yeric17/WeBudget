

using WebAPI.Domain.Users;

namespace Domain.Users.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateRefreshToken();
        Task<string> GenerateTokenAsync(User user);
    }
}

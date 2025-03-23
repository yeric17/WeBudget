using WebAPI.Domain.Users.Entities;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Domain.Users.Interfaces
{
    public interface IUsersRepository
    {
        Task<Result> AddRefreshToken(RefreshToken refreshToken);
        Task<Result<RefreshToken>> GetRefreshToken(string refreshToken);
    }
}

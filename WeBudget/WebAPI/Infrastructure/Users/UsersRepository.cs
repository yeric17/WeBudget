using Microsoft.EntityFrameworkCore;
using WebAPI.Domain.Users;
using WebAPI.Domain.Users.Entities;
using WebAPI.Domain.Users.Interfaces;
using WebAPI.Infrastructure.Common;
using WebAPI.Infrastructure.Common.Abstractions;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Infrastructure.Users
{
    internal class UsersRepository : EFCoreRepository, IUsersRepository
    {
        private readonly ILogger<UsersRepository> _logger;
        public UsersRepository(ApplicationDbContext dbContext, ILogger<UsersRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<Result<RefreshToken>> GetRefreshToken(string refreshToken)
        {
            RefreshToken? token = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (token is null)
            {
                return UserErrors.RefreshTokenNotFound;
            }

            return token;
        }

        public async Task<Result> AddRefreshToken(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);

            try
            {
                await _dbContext.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception exc)
            {

                _logger.LogError(exc, "Error while adding refresh token to database");

                return UserErrors.DatabasUnhandeledError;
            }
         
        }

        public async Task<Result<User>> GetUserById(string userId)
        {
            User? user = await _dbContext.Users.FindAsync(userId);
            if (user is null)
            {
                return UserErrors.UserNotFound;
            }
            return user;
        }
    }
}

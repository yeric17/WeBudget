using Domain.Users.Interfaces;
using MediatR;
using WebAPI.Domain.Users;
using WebAPI.Domain.Users.Entities;
using WebAPI.Domain.Users.Interfaces;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Application.Users
{
    public sealed class RefreshTokenGetQueryHandler : IRequestHandler<RefreshTokenGetQuery, Result<UserLoginCommandResponse>>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public RefreshTokenGetQueryHandler(IUsersRepository usersRepository, ITokenGenerator tokenGenerator)
        {
            _usersRepository = usersRepository;
            _tokenGenerator = tokenGenerator;
        }
        public async  Task<Result<UserLoginCommandResponse>> Handle(RefreshTokenGetQuery request, CancellationToken cancellationToken)
        {
            Result<RefreshToken> result = await _usersRepository.GetRefreshToken(request.Token);

            if (result.IsFailure)
            {
                return result.Error;
            }


            RefreshToken oldRefreshToken = result.Value;

            if (!oldRefreshToken.IsActive)
            {
                return UserErrors.UserUnauthorized;
            }

            string newRefreshTokenString = _tokenGenerator.GenerateRefreshToken();

            Result<User> userResult = await _usersRepository.GetUserById(oldRefreshToken.UserId);

            if (userResult.IsFailure)
            {
                return userResult.Error;
            }


            await _usersRepository.DisableRefreshToken(oldRefreshToken);

            User user = userResult.Value;

            string token = await _tokenGenerator.GenerateTokenAsync(user);


            RefreshToken refreshToken = RefreshToken
            .Create(newRefreshTokenString, user.Id)
            .WithExpires(TimeSpan.FromMinutes(30));

            var refreshTokenResult =  await _usersRepository.AddRefreshToken(refreshToken);

            if(refreshTokenResult.IsFailure)
            {
                return refreshTokenResult.Error;
            }


            UserLoginCommandResponse response = new UserLoginCommandResponse
            {
                AccessToken = token,
                RefreshToken = newRefreshTokenString
            };

            return response;

        }
    }

}

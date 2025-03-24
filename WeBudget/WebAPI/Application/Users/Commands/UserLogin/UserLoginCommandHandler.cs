using Domain.Users.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPI.Domain.Users;
using WebAPI.Domain.Users.Entities;
using WebAPI.Domain.Users.Interfaces;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Application.Users
{
    public sealed class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<UserLoginCommandResponse>>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserLoginCommandHandler> _logger;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUsersRepository _usersRepository;
        public UserLoginCommandHandler(IServiceProvider serviceProvider, ILogger<UserLoginCommandHandler> logger, ITokenGenerator tokenGenerator, IUsersRepository usersRepository)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
            _usersRepository = usersRepository;
        }
        public async Task<Result<UserLoginCommandResponse>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            UserManager<User> userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            User? user = await userManager.FindByEmailAsync(request.Email);

            if(user is null)
            {
                _logger.LogError($"The user {request.Email} not found");
                return UserErrors.UserUnauthorized;
            }

            bool passwordMatch = await userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordMatch)
            {
                _logger.LogError($"The password for the user {request.Email} is incorrect");
                return UserErrors.UserUnauthorized;
            }

            string? accessToken = await _tokenGenerator.GenerateTokenAsync(user);

            if (accessToken is null)
            {
                _logger.LogError($"The access token for the user {request.Email} could not be generated");
                return UserErrors.UserUnauthorized;
            }

            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            RefreshToken token = RefreshToken
                .Create(refreshToken,user.Id)
                .WithExpires(TimeSpan.FromMinutes(30));

            Result result = await _usersRepository.AddRefreshToken(token);


            if (result.IsFailure)
            {
                _logger.LogError($"The refresh token for the user {request.Email} could not be saved");
                return result.Error;
            }

            return new UserLoginCommandResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }
    }
}

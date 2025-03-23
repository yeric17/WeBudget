using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPI.Domain.Users;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Application.Users
{
    public sealed class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand,Result>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserRegisterCommandHandler> _logger;

        public UserRegisterCommandHandler(IServiceProvider serviceProvider, ILogger<UserRegisterCommandHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<Result> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {

            UserManager<User> userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            User? userExists = await userManager.FindByEmailAsync(request.Email);

            List<Error> errors = new();

            if (userExists is not null)
            {
                _logger.LogError("User already exists.");
                errors.Add(UserErrors.UserAlreadyExists);
                return errors;
            }

            User user = User.Create(
                request.Nickname,
                request.Email
            );


            IdentityResult? result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                
                _logger.LogError("User can not be created. Errors: {Errors}", result.Errors);
                return UserErrors.UserIdentityErros(result.Errors);
            }

            User? userStored = await userManager.FindByEmailAsync(request.Email);

            if (userStored is null)
            {
                _logger.LogError("User not found after creation.");
                errors.Add(UserErrors.UserNotFound);
                return errors;
            }

            await userManager.AddToRoleAsync(userStored, "User");


            return Result.Success();
        }
    }
}

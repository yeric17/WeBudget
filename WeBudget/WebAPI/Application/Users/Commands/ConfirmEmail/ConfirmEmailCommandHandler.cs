using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebAPI.Domain.Users;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Application.Users
{
    public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        private readonly IConfiguration _configuration;
        public ConfirmEmailCommandHandler(IServiceProvider serviceProvider, ILogger<ConfirmEmailCommandHandler> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            UserManager<User> userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            if (await userManager.FindByEmailAsync(request.Email) is not { } user)
            {
                // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
                return UserErrors.UserUnauthorized;
            }

            string code = request.Token;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {
                return UserErrors.UserUnauthorized;
            }

            IdentityResult result = await userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                _logger.LogError("Error confirming email for user {UserId}. Errors: {Errors}", user.Id, result.Errors);
                return UserErrors.UserIdentityErros(result.Errors);
            }

            return Result.Success();

        }
    }
}

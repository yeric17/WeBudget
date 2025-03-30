using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebAPI.Domain.Users;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Application.Users
{
    public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        public ResetPasswordCommandHandler(IServiceProvider serviceProvider, ILogger<ResetPasswordCommandHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            UserManager<User> userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is not null)
            {
                var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                var result = await userManager.ResetPasswordAsync(user, decodedCode, request.Password);
                if (result.Succeeded)
                {
                    return Result.Success();
                }
                else
                {
                    _logger.LogError("Password reset failed. Errors: {Errors}", result.Errors);
                    return UserErrors.UserIdentityErros(result.Errors);
                }
            }
            return UserErrors.UserUnauthorized;
        }
    }

}

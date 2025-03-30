using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using WebAPI.Domain.Users;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;

namespace WebAPI.Application.Users
{
    public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSender<User> _emailSender;
        public ForgotPasswordCommandHandler(IServiceProvider serviceProvider, IEmailSender<User> emailSender)
        {
            _serviceProvider = serviceProvider;
            _emailSender = emailSender;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            UserManager<User> userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(request.Email);


            if (user is not null && await userManager.IsEmailConfirmedAsync(user))
            {

                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                await _emailSender.SendPasswordResetCodeAsync(user, request.Email, HtmlEncoder.Default.Encode(code));

                return Result.Success();
            }

            return UserErrors.UserUnauthorized;

        }
    }
}

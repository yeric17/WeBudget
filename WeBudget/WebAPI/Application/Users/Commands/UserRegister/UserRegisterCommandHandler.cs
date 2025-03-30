using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebAPI.Domain.Users;
using WebAPI.Shared.Abstractions;
using WebAPI.Shared.Errors;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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
            LinkGenerator linkGenerator = _serviceProvider.GetRequiredService<LinkGenerator>();
            IHttpContextAccessor httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            HttpContext? httpContext = httpContextAccessor.HttpContext;

            if(httpContext is null)
            {
                _logger.LogError("HttpContext is null");
                return UserErrors.HttpContextIsNull;
            }

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


            string token = await userManager.GenerateEmailConfirmationTokenAsync(userStored);

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var routeValues = new RouteValueDictionary()
            {
                ["email"] = request.Email,
                ["token"] = encodedToken,
            };

            if(request.ConfirmationLink is null)
            {
                _logger.LogError("Confirmation link is null.");
                errors.Add(UserErrors.ConfirmationLinkIsNull);
                return errors;
            }


            string? confirmationLink = linkGenerator.GetUriByName(httpContext, request.ConfirmationLink, routeValues);

            if(confirmationLink is null)
            {
                _logger.LogError("Confirmation link is null.");
                errors.Add(UserErrors.ConfirmationLinkIsNull);
                return errors;
            }

            IEmailSender<User> emailSender = _serviceProvider.GetRequiredService<IEmailSender<User>>();

            await emailSender.SendConfirmationLinkAsync(userStored, request.Email, confirmationLink);


            return Result.Success();
        }
    }
}

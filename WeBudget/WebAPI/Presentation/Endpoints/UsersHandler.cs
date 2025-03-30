using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAPI.Application.Users;
using WebAPI.Infrastructure;
using WebAPI.Infrastructure.Email;
using WebAPI.Presentation.Utils;

namespace WebAPI.Presentation.Endpoints
{
    public static class UsersHandler
    {
        private static string confirmEmailEndpointName = string.Empty;
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder routes)
        {
            routes.MapPost("/register", Register).WithDescription("Register User");
            routes.MapPost("/login", Login).WithDescription("Login");
            routes.MapGet("/me", Me).RequireAuthorization().WithDescription("Get current user");
            routes.MapPost("/refresh-token", RefreshToken).WithDescription("Refresh token");
            routes.MapGet("/confirm-email", ConfirmEmail)
                .WithDescription("Confirm email")
                .Add(endpointBuilder =>
                {
                    var finalPattern = ((RouteEndpointBuilder)endpointBuilder).RoutePattern.RawText;
                    confirmEmailEndpointName = $"{nameof(MapUsers)}-{finalPattern}";
                    endpointBuilder.Metadata.Add(new EndpointNameMetadata(confirmEmailEndpointName));
                });
            return routes;
        }

        public static async Task<IResult> Register([FromServices] ISender sender, [FromBody] UserRegisterCommand request)
        {
            var result = await sender.Send(request with { ConfirmationLink = confirmEmailEndpointName});

            if (result.IsFailure)
            {
               return ProblemDetailsHelper.HandleFailure(result);
            }
            return Results.Ok();
        }

        public static async Task<IResult> Login([FromServices] ISender sender, [FromBody] UserLoginCommand request)
        {
            var result = await sender.Send(request);
            if (result.IsFailure)
            {
                return ProblemDetailsHelper.HandleFailure(result);
            }
            return Results.Ok(result.Value);
        }

        public static async Task<IResult> Me([FromServices] ISender sender,[FromServices] IClaimsHelper claimsHelper )
        {
            string? userId = claimsHelper.GetUserId();

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new UserGetByIdQuery { UserId = userId});

            if (result.IsFailure)
            {
                return ProblemDetailsHelper.HandleFailure(result);
            }
            return Results.Ok(result.Value);
        }

        public static async Task<IResult> RefreshToken([FromServices] ISender sender, [FromBody] RefreshTokenGetQuery request)
        {
            var result = await sender.Send(request);
            if (result.IsFailure)
            {
                return ProblemDetailsHelper.HandleFailure(result);
            }
            return Results.Ok(result.Value);
        }

        public static async Task<IResult> ConfirmEmail([FromServices] ISender sender, [AsParameters] ConfirmEmailCommand request, [FromServices] IOptions<EmailTemplateSettings> options)
        {
            var result = await sender.Send(request);

            if (result.IsFailure)
            {
                return ProblemDetailsHelper.HandleFailure(result);
            }

            EmailTemplateSettings emailTemplateSettings = options.Value;

            if (emailTemplateSettings.ConfirmAccount.ClientUrl is not null)
            {
                return TypedResults.Redirect(emailTemplateSettings.ConfirmAccount.ClientUrl);
            }

            return Results.Ok();
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Users;
using WebAPI.Presentation.Utils;

namespace WebAPI.Presentation.Endpoints
{
    public static class UsersHandler
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder routes)
        {
            routes.MapPost("/register", Register).WithDescription("Register User");
            routes.MapPost("/login", Login).WithDescription("Login");
            return routes;
        }

        public static async Task<IResult> Register([FromServices] ISender sender, [FromBody] UserRegisterCommand request)
        {
            var result = await sender.Send(request);

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

    }
}

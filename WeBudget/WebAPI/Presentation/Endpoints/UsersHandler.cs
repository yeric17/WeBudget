﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Users;
using WebAPI.Infrastructure;
using WebAPI.Presentation.Utils;

namespace WebAPI.Presentation.Endpoints
{
    public static class UsersHandler
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder routes)
        {
            routes.MapPost("/register", Register).WithDescription("Register User");
            routes.MapPost("/login", Login).WithDescription("Login");
            routes.MapGet("/me", Me).RequireAuthorization().WithDescription("Get current user");
            routes.MapPost("/refresh-token", RefreshToken).WithDescription("Refresh token");
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
    }
}

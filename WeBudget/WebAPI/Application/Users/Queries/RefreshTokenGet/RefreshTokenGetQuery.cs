using MediatR;
using WebAPI.Domain.Users.Entities;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public record RefreshTokenGetQuery : IRequest<Result<UserLoginCommandResponse>>
    {
        public string Token { get; init; } = string.Empty;
    }

}

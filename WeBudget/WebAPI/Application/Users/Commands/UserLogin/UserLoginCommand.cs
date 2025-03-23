using MediatR;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public record UserLoginCommand : IRequest<Result<UserLoginCommandResponse>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}

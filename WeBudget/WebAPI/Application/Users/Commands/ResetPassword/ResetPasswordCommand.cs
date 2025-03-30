using MediatR;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public record ResetPasswordCommand : IRequest<Result>
    {
        public string Code { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }

}

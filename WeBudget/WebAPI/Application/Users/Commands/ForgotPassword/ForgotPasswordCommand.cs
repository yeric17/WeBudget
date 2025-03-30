using MediatR;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public record ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; init; } = string.Empty;
    }
}

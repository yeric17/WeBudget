using MediatR;
using WebAPI.Shared.Abstractions;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace WebAPI.Application.Users
{
    public record ConfirmEmailCommand : IRequest<Result>
    {
        public string Token { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }
}

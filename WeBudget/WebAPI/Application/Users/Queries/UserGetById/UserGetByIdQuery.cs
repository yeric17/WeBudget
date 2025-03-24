using MediatR;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public record UserGetByIdQuery : IRequest<Result<UserGetByIdQueryResponse>>
    {
        public string UserId { get; init; } = string.Empty;
    }
}

using MediatR;
using WebAPI.Domain.Users.Interfaces;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Application.Users
{
    public sealed class UserGetByIdQueryHandler : IRequestHandler<UserGetByIdQuery, Result<UserGetByIdQueryResponse>>
    {
        private readonly IUsersRepository _usersRepository;
        public UserGetByIdQueryHandler(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }
        public async Task<Result<UserGetByIdQueryResponse>> Handle(UserGetByIdQuery request, CancellationToken cancellationToken)
        {
            var userResult = await _usersRepository.GetUserById(request.UserId);
            return userResult.IsSuccess
                ? Result.Success(new UserGetByIdQueryResponse(userResult.Value.Id, userResult.Value.Email, userResult.Value.UserName))
                : Result.Failure<UserGetByIdQueryResponse>(userResult.Error);
        }
    }
}

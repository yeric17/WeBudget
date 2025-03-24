using FluentValidation;

namespace WebAPI.Application.Users
{
    public sealed class UserGetByIdQueryValidator : AbstractValidator<UserGetByIdQuery>
    {
        public UserGetByIdQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}

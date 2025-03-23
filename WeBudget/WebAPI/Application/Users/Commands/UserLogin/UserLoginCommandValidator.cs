using FluentValidation;

namespace WebAPI.Application.Users.Commands.UserLogin
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .MinimumLength(4)
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}

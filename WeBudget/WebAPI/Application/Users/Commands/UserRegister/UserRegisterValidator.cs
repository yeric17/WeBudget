using FluentValidation;

namespace WebAPI.Application.Users
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Nickname)
                .NotEmpty()
                .MinimumLength(4)
                .MaximumLength(50);
            RuleFor(x => x.Email)
                .NotEmpty()
                .MinimumLength(4)
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}

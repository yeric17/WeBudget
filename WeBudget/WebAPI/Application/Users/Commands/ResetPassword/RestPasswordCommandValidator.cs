using FluentValidation;

namespace WebAPI.Application.Users
{
    public class RestPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public RestPasswordCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty();
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}

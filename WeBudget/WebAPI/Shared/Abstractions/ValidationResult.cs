namespace WebAPI.Shared.Abstractions;

public sealed class ValidationResult : Result, IValidationResult
{
    private ValidationResult(List<Error> errors)
        : base(false, IValidationResult.ValidationError ) =>
        Errors = errors;


    public static ValidationResult WithErrors(List<Error> errors) => new(errors);
}

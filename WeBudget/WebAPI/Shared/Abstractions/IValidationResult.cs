namespace WebAPI.Shared.Abstractions;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "ValidationError",
        "A validation problem occurred.", ApplicationErrorType.InvalidValue);

    public List<Error> Errors { get; }

}

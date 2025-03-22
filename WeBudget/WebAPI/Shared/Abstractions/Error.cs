using Microsoft.AspNetCore.Identity;

namespace WebAPI.Shared.Abstractions;

public class Error : IEquatable<Error>
{
    public static readonly Error None = new(string.Empty, string.Empty, ApplicationErrorType.Unknown);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", ApplicationErrorType.NullValue);
    public static Error NotFound(string code,  string message) => new(code, message, ApplicationErrorType.NotFound);
    public static Error InvalidValue(string code, string message) => new(code, message, ApplicationErrorType.InvalidValue);
    public static Error Unauthorized(string code, string message) => new(code, message, ApplicationErrorType.Unauthorized);
    public static Error Forbidden(string code, string message) => new(code, message, ApplicationErrorType.Forbidden);
    public static Error Conflict(string code, string message) => new(code, message, ApplicationErrorType.Conflict);
    public static Error InvalidOperation(string code, string message) => new(code, message, ApplicationErrorType.InvalidOperation);
    public static Error ServiceUnavailable(string code, string message) => new(code, message, ApplicationErrorType.ServiceUnavailable);
    public static Error Unknown(string code, string message) => new(code, message, ApplicationErrorType.Unknown);

    public static List<Error> InvalidValue(IEnumerable<IdentityError> errors)
    {
        return errors.Select(x => InvalidValue(x.Code, x.Description)).ToList();
    }

    public Error(string code, string message, ApplicationErrorType errorType)
    {
        Code = code;
        Message = message;
        ErrorType = errorType;
    }



    public string Code { get; }

    public string Message { get; }

    public ApplicationErrorType ErrorType { get; }

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b) => !(a == b);

    public virtual bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message && other.ErrorType == ErrorType;
    }

    public override bool Equals(object? obj) => obj is Error error && Equals(error);

    public override int GetHashCode() => HashCode.Combine(Code, Message, ErrorType);

    public override string ToString() => Code;
}


public enum ApplicationErrorType
{
    None,
    NullValue,
    InvalidValue,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    InvalidOperation,
    ServiceUnavailable,
    Unknown
}
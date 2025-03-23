using Microsoft.AspNetCore.Identity;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Shared.Errors
{
    public static class UserErrors
    {
        public static Error UserNotFound = Error.NotFound("UserNotFound", "The specified user was not found.");
        public static Error UserCanNotCreate = Error.InvalidOperation("UserCanNotCreate", "The user can not be created.");
        public static Error UserAlreadyExists = Error.Conflict("UserAlreadyExists", "The specified user already exists.");
        public static List<Error> UserIdentityErros(IEnumerable<IdentityError> errors) => Error.InvalidValue(errors);

        public static Error UserUnauthorized = Error.Unauthorized("UserUnauthorized", "The user is not authorized.");
        public static Error RefreshTokenNotFound = Error.NotFound("RefreshTokenNotFound", "The specified refresh token was not found.");
        public static Error RefreshTokenExpired = Error.InvalidValue("RefreshTokenExpired", "The specified refresh token has expired.");

        public static Error DatabasUnhandeledError = Error.Unknown("DatabasUnhandeledError", "An unhandled error occurred while processing the request.");
    }
}

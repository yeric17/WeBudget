using Microsoft.AspNetCore.Identity;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Shared.Errors
{
    public static class UserErrors
    {
        public static readonly Error UserNotFound = Error.NotFound("UserNotFound", "The specified user was not found.");
        public static readonly Error UserCanNotCreate = Error.InvalidOperation("UserCanNotCreate", "The user can not be created.");
        public static readonly Error UserAlreadyExists = Error.Conflict("UserAlreadyExists", "The specified user already exists.");
        public static List<Error> UserIdentityErros(IEnumerable<IdentityError> errors) => Error.InvalidValue(errors);

        public static readonly Error UserUnauthorized = Error.Unauthorized("UserUnauthorized", "The user is not authorized.");
        public static readonly Error RefreshTokenNotFound = Error.NotFound("RefreshTokenNotFound", "The specified refresh token was not found.");
        public static readonly Error RefreshTokenExpired = Error.InvalidValue("RefreshTokenExpired", "The specified refresh token has expired.");

        public static readonly Error DatabasUnhandeledError = Error.Unknown("DatabasUnhandeledError", "An unhandled error occurred while processing the request.");

        public static readonly Error ConfirmationLinkIsNull = Error.Conflict("ConfirmationLinkIsNull", "The confirmation link is null.");

        public static readonly Error HttpContextIsNull = Error.Conflict("HttpContextIsNull", "The http context is null");
    }
}

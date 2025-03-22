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
    }
}

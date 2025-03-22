using Microsoft.AspNetCore.Mvc;
using WebAPI.Shared.Abstractions;

namespace WebAPI.Presentation.Utils
{
    public static class ProblemDetailsHelper
    {

        public static IResult HandleFailure(Result result)
        {
            return result switch
            {
                IValidationResult validationResult => Results.Problem(CreateProblemDetails("Validation Error", StatusCodes.Status400BadRequest,IValidationResult.ValidationError, validationResult.Errors)),
                _ => result.Error.ErrorType switch
                {
                    ApplicationErrorType.NotFound => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status404NotFound, result.Error)),
                    ApplicationErrorType.InvalidValue => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status400BadRequest, result.Error)),
                    ApplicationErrorType.NullValue => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status400BadRequest, result.Error)),
                    ApplicationErrorType.Unauthorized => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status401Unauthorized, result.Error)),
                    ApplicationErrorType.Conflict => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status409Conflict, result.Error)),
                    ApplicationErrorType.Unknown => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status500InternalServerError, result.Error)),
                    ApplicationErrorType.ServiceUnavailable => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status503ServiceUnavailable, result.Error)),
                    ApplicationErrorType.Forbidden => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status403Forbidden, result.Error)),
                    ApplicationErrorType.InvalidOperation => Results.Problem(CreateProblemDetails(result.Error.Code, StatusCodes.Status400BadRequest, result.Error)),
                    _ => Results.Problem(CreateProblemDetails("Internal Server Error", StatusCodes.Status500InternalServerError, result.Error))
                }
            };
        }
        public static ProblemDetails CreateProblemDetails(string title, int status, Error error, List<Error> errors)
        {
            return new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = error.Message,
                Type = error.Code,
                Extensions = {{ nameof(errors), errors }}
            };
        }

        public static ProblemDetails CreateProblemDetails(string title, int status, Error error)
        {
            return new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = error.Message,
                Type = error.Code
            };
        }
    }
}

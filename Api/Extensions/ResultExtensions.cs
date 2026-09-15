using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToApiResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        if (result.Error is null)
        {
            return new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed.",
                Detail = result.Error.Message
            })
            {
                StatusCode = StatusCodes.Status400BadRequest
            },

            ErrorType.Conflict => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict.",
                Detail = result.Error.Message
            })
            {
                StatusCode = StatusCodes.Status409Conflict
            },

            ErrorType.NotFound => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found.",
                Detail = result.Error.Message
            })
            {
                StatusCode = StatusCodes.Status404NotFound
            },

            ErrorType.Unauthorized => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized.",
                Detail = result.Error.Message
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            },

            ErrorType.Forbidden => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden.",
                Detail = result.Error.Message
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            },

            _ => new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = result.Error.Message
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };
    }
}
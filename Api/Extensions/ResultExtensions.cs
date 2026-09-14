using Domain.Abstractions;

namespace Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToApiResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        if (result.Error is null)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.");
        }

        return result.Error.Type switch
        {
            ErrorType.Validation => TypedResults.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation failed.",
                detail: result.Error.Message),

            ErrorType.Conflict => Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict.",
                detail: result.Error.Message),

            ErrorType.NotFound => TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found.",
                detail: result.Error.Message),

            ErrorType.Unauthorized => TypedResults.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized.",
                detail: result.Error.Message),

            ErrorType.Forbidden => TypedResults.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden.",
                detail: result.Error.Message),

            _ => TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: result.Error.Message)
        };
    }
}

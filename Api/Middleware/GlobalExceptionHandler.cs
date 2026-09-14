namespace Api.Middleware;

using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public sealed class GlobalExceptionHandler(
    IWebHostEnvironment env,
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case ValidationException validationException:
                var errors = validationException.Errors
                                .Select(x => $"{x.ErrorCode}: {x.ErrorMessage}")
                                .ToList();

                problemDetails.Status =
                    StatusCodes.Status422UnprocessableEntity;

                problemDetails.Title = "Validation Error";

                problemDetails.Detail = string.Join(" | ", errors);

                break;

            case UnauthorizedAccessException:
                problemDetails.Status =
                 StatusCodes.Status401Unauthorized;

                problemDetails.Title = "Unauthorized";

                problemDetails.Detail =
                    "You are not authorized to perform this action.";

                break;

            case BadHttpRequestException:
                problemDetails.Status = StatusCodes.Status400BadRequest;

                problemDetails.Title = "Bad Request";

                problemDetails.Detail =
                    "The request was invalid.";

                break;

            case DbUpdateException dbUpdateException:
                problemDetails.Status = StatusCodes.Status409Conflict;

                problemDetails.Title = "Database Error";

                problemDetails.Detail =
                    env.IsDevelopment()
                        ? dbUpdateException.InnerException?.Message
                        : "A database error occurred while processing your request.";

                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;

                problemDetails.Title = "Internal Server Error";

                problemDetails.Detail =
                    env.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred.";

                break;
        }

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
    }
}
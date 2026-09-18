using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ValidationExtensions
{
    public static IActionResult ToValidationApiResponse(
        this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(error =>
                char.ToLowerInvariant(error.PropertyName[0]) + error.PropertyName[1..])
            .ToDictionary(
                group => group.Key,
                group => group.Select(e => e.ErrorMessage)
                    .Distinct()
                    .ToArray()
            );

        var details = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = "One or more validation errors occurred."
        };

        details.Extensions["code"] = "Validation.Failed";

        return new BadRequestObjectResult(details);

    }
}

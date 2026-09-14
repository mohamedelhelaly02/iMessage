using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public sealed class ApiKeyMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    IProblemDetailsService problemDetailsService)
{
    private const string ApiKeyHeaderName = "X-API-KEY";
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Get API Key from request header
        if (!context.Request.Headers.TryGetValue(
                ApiKeyHeaderName,
                out var extractedApiKey))
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "API Key is missing from the request headers.");

            return;
        }

        // 2. Get configured API Key
        var configuredApiKey = configuration["ApiKey"];

        if (string.IsNullOrWhiteSpace(configuredApiKey))
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "API Key is not configured in the application settings.");

            return;
        }

        // 3. Validate API Key
        if (!string.Equals(
                configuredApiKey,
                extractedApiKey.ToString(),
                StringComparison.Ordinal))
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "The provided API Key is invalid.");

            return;
        }

        // 4. API Key is valid
        await next(context);
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string details)
    {
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = details
        };

        await problemDetailsService.WriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problemDetails
            });
    }
}

internal static class ApiKeyMiddlewareExtensions
{
    extension(IApplicationBuilder builder)
    {
        public IApplicationBuilder UseApiKeyMiddleware()
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
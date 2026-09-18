using Microsoft.OpenApi;

namespace Api.Extensions;

public static class OpenApiExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOpenApiDoc()
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "iMessage API",
                        Version = "v1",
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    //document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
                    //{
                    //    Name = "X-API-KEY",
                    //    Type = SecuritySchemeType.ApiKey,
                    //    In = ParameterLocation.Header,
                    //    Description = "API key needed to access the endpoints. Add 'X-API-KEY' header with your API key."
                    //};
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        In = ParameterLocation.Header,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Bearer token"
                    };
                    document.Security ??= [];
                    //document.Security.Add(new OpenApiSecurityRequirement
                    //{
                    //    [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
                    //});
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                    return Task.CompletedTask;
                });

            });

            return services;
        }
    }
}

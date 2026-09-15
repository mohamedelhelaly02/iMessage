using Application.Behaviors;
using Application.Features.Auth.Register;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplication()
            {
                var assembly = typeof(RegisterCommand).Assembly;

                services.AddValidatorsFromAssembly(assembly);

                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                });

                return services;
            }
        }
    }
}

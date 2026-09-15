using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Identity.Jwt;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddInfrastructure(IConfiguration configuration)
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"), c => c.EnableRetryOnFailure()));

                services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

                var jwtSettings = new JwtSettings();

                configuration.GetSection(nameof(JwtSettings)).Bind(jwtSettings);

                services.AddSingleton(jwtSettings);

                services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;
                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var path = context.HttpContext.Request.Path;

                                if (!path.StartsWithSegments("/hubs"))
                                    return Task.CompletedTask;

                                var accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken))
                                    context.Token = accessToken;

                                return Task.CompletedTask;
                            },

                            OnAuthenticationFailed = context =>
                            {
                                Console.WriteLine($"[JWT] Auth failed: {context.Exception.GetType().Name} - {context.Exception.Message}");
                                return Task.CompletedTask;
                            },

                            OnTokenValidated = context =>
                            {
                                Console.WriteLine($"[JWT] Token validated for path: {context.HttpContext.Request.Path}");
                                return Task.CompletedTask;
                            }
                        };
                    });

                services.AddAuthorization();

                services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();


                services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
                services.AddScoped<ICurrentUserService, CurrentUserService>();

                return services;
            }
        }
    }
}

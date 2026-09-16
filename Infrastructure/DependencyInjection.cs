using Application.Interfaces;
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

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.SaveToken = true;

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidIssuer = configuration["JwtSettings:Issuer"],
                            ValidAudience = configuration["JwtSettings:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    configuration.GetValue<string>("JwtSettings:Key")!)),
                            ClockSkew = TimeSpan.Zero
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var path = context.HttpContext.Request.Path;

                                if (path.StartsWithSegments("/hubs"))
                                {
                                    var accessToken = context.Request.Query["access_token"];

                                    if (!string.IsNullOrEmpty(accessToken))
                                        context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });

                services.AddAuthorization();

                services.AddIdentity<ApplicationUser, IdentityRole<string>>(options =>
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
                services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

                return services;
            }
        }
    }
}

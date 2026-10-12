using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Application.Behaviors;
using server.Application.Interfaces;
using server.Domain.Entities;
using server.Infrastructure.Identity;
using server.Infrastructure.Persistence;
using server.Shared.Abstraction;
using server.Shared.Extensions;
using server.Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"), c => c.EnableRetryOnFailure()));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .Validate(settings =>
    {
        return !string.IsNullOrWhiteSpace(settings.Secret) &&
               !string.IsNullOrWhiteSpace(settings.Issuer) &&
               !string.IsNullOrWhiteSpace(settings.Audience) &&
               settings.ExpirationInMinutes > 0 &&
               settings.RefreshTokenExpirationInDays > 0;
    }, "Invalid JWT configuration.")
    .ValidateOnStart();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();

app.UseAuthorization();

RouteGroupBuilder group = app.MapGroup("api/");

app.MapEndpoints(group);

app.Run();
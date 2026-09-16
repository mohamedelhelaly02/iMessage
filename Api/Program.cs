using Api.Extensions;
using Api.Hubs;
using Api.Middleware;
using Application;
using Infrastructure;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options
        .JsonSerializerOptions
        .Converters
        .Add(new JsonStringEnumConverter()));


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDoc();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
    options.AddPolicy("Angular",
        policy =>
            policy.WithOrigins(builder.Configuration["ClientUri"]!)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}


app.UseExceptionHandler(o => { });

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("Angular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
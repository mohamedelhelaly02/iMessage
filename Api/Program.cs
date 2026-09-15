using Api.Extensions;
using Api.Hubs;
using Api.Middleware;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDoc();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
           .WithOrigins(builder.Configuration["ClientUri"]!)
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "iMessage"));
}


app.UseCors("Angular");

app.UseExceptionHandler(o => { });

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Campus Header-Based Auth Middleware
app.Use(async (context, next) =>
{
    var email = context.Request.Headers["x-forwarded-user"].FirstOrDefault();
    var name = context.Request.Headers["x-forwarded-name"].FirstOrDefault();

    if (!string.IsNullOrEmpty(email))
    {
        context.Items["UserEmail"] = email;
        context.Items["UserName"] = name;
    }

    await next();
});

app.MapGet("/api/health", () => Results.Ok("API running"));

app.MapGet("/api/me", (HttpContext context) =>
{
    return Results.Ok(new {
        email = context.Items["UserEmail"],
        name = context.Items["UserName"]
    });
});

app.Run();

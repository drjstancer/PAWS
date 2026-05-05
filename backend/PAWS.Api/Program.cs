using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using PAWS.Api.Data;
using PAWS.Api.Security;
using PAWS.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PawsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<AuthMiddleware>();

app.MapGet("/api/health", () => Results.Ok("API running"));

app.MapGet("/api/v1/me", (ICurrentUserService currentUser) =>
{
    return Results.Ok(currentUser.User);
});

app.Run();

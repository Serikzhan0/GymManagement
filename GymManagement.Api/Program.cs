using GymManagement.Api.Middleware;
using GymManagement.Application;
using GymManagement.Application.Common.Models;
using GymManagement.Application.Interfaces;
using GymManagement.Infrastructure;
using GymManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var firstErrorMessage = errors.Values.FirstOrDefault()?.FirstOrDefault() ?? "Validation failed.";
        var response = new ErrorResponse(StatusCodes.Status400BadRequest, firstErrorMessage, errors);
        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GymManagementApi",
        Version = "v1",
        Description = "Gym Management REST API with JWT Authentication and Role-Based Authorization."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    var schemeReference = new OpenApiSecuritySchemeReference("Bearer");
    c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        { schemeReference, new List<string>() }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GymManagementApi v1");
    c.RoutePrefix = string.Empty; // Serves Swagger at the root URL
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto-migrate and seed database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = services.GetRequiredService<GymManagementDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        await DbInitializer.SeedAsync(dbContext, passwordHasher, logger);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Could not run database migration/seed on startup (database might not be running yet).");
    }
}

app.Run();

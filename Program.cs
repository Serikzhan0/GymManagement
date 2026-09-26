using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using GymManagementApi;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Controllers & JSON settings
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 2. Consistent validation error response format
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

// 3. Database configuration (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. JWT Service & Authentication
builder.Services.AddScoped<JwtService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForGymManagementApiExamTesting12345!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GymManagementApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GymManagementApi";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// 5. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 6. Swagger with JWT Authentication
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GymManagementApi",
        Version = "v1",
        Description = "Gym Management REST API with JWT Authentication"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter token directly.",
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

// 7. Global Exception Handling Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var errorResponse = new ErrorResponse(
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred. Please try again later."
        );

        var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
});

// 8. Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GymManagementApi v1");
    c.RoutePrefix = string.Empty; // Open Swagger UI at root "/"
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 9. Database Migration and Seeder on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync();
        }

        // Seed initial data if database is empty
        if (!await db.Users.AnyAsync())
        {
            logger.LogInformation("Seeding initial exam data...");

            // 1. Users
            var adminUser = new User
            {
                Name = "Admin User",
                Email = "admin@gym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };

            var trainerUser = new User
            {
                Name = "Mike Trainer",
                Email = "trainer@gym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Trainer123!"),
                Role = UserRole.Trainer,
                CreatedAt = DateTime.UtcNow
            };

            var normalUser = new User
            {
                Name = "John Member",
                Email = "user@gym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.AddRange(adminUser, trainerUser, normalUser);
            await db.SaveChangesAsync();

            // 2. Trainers
            var trainer1 = new Trainer
            {
                Name = "Mike Trainer",
                Specialization = "Strength & Conditioning",
                ExperienceYears = 5
            };

            var trainer2 = new Trainer
            {
                Name = "Sarah Coach",
                Specialization = "Cardio & HIIT",
                ExperienceYears = 3
            };

            db.Trainers.AddRange(trainer1, trainer2);
            await db.SaveChangesAsync();

            // 3. Exercises
            var benchPress = new Exercise
            {
                Name = "Bench Press",
                Description = "Barbell flat bench press",
                MuscleGroup = "Chest"
            };

            var squat = new Exercise
            {
                Name = "Barbell Squat",
                Description = "Back squat below parallel",
                MuscleGroup = "Legs"
            };

            var pullUp = new Exercise
            {
                Name = "Pull-Up",
                Description = "Wide grip bodyweight pull-up",
                MuscleGroup = "Back"
            };

            var overheadPress = new Exercise
            {
                Name = "Overhead Press",
                Description = "Standing barbell overhead shoulder press",
                MuscleGroup = "Shoulders"
            };

            db.Exercises.AddRange(benchPress, squat, pullUp, overheadPress);
            await db.SaveChangesAsync();

            // 4. Workout Plan
            var plan = new WorkoutPlan
            {
                Name = "Full Body Starter",
                Description = "Foundational full body strength routine",
                TrainerId = trainer1.Id,
                CreatedAt = DateTime.UtcNow,
                WorkoutExercises = new List<WorkoutExercise>
                {
                    new() { ExerciseId = benchPress.Id, Sets = 3, Repetitions = 10 },
                    new() { ExerciseId = squat.Id, Sets = 3, Repetitions = 8 },
                    new() { ExerciseId = pullUp.Id, Sets = 3, Repetitions = 8 }
                }
            };

            db.WorkoutPlans.Add(plan);

            // 5. Active Membership for John Member
            var membership = new Membership
            {
                UserId = normalUser.Id,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(25),
                Type = MembershipType.Monthly,
                Status = MembershipStatus.Active
            };

            db.Memberships.Add(membership);

            await db.SaveChangesAsync();
            logger.LogInformation("Database seeded successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Could not run database migration/seed on startup (database might not be ready yet).");
    }
}

app.Run();

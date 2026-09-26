using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagement.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(GymManagementDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        try
        {
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Seeding initial database data...");

                // 1. Seed Users
                var admin = new User
                {
                    Name = "Admin User",
                    Email = "admin@gym.com",
                    PasswordHash = passwordHasher.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                };

                var trainerUser = new User
                {
                    Name = "Mike Trainer",
                    Email = "trainer@gym.com",
                    PasswordHash = passwordHasher.HashPassword("Trainer123!"),
                    Role = UserRole.Trainer,
                    CreatedAt = DateTime.UtcNow
                };

                var regularUser = new User
                {
                    Name = "John Member",
                    Email = "user@gym.com",
                    PasswordHash = passwordHasher.HashPassword("User123!"),
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.AddRange(admin, trainerUser, regularUser);
                await context.SaveChangesAsync();

                // 2. Seed Trainers
                var trainer = new Trainer
                {
                    Name = "Mike Trainer",
                    Specialization = "Strength and Conditioning",
                    ExperienceYears = 6
                };

                var trainer2 = new Trainer
                {
                    Name = "Sarah Coach",
                    Specialization = "CrossFit & Mobility",
                    ExperienceYears = 4
                };

                context.Trainers.AddRange(trainer, trainer2);
                await context.SaveChangesAsync();

                // 3. Seed Exercises
                var benchPress = new Exercise
                {
                    Name = "Bench Press",
                    Description = "Barbell chest press on flat bench",
                    MuscleGroup = "Chest"
                };

                var squat = new Exercise
                {
                    Name = "Barbell Squat",
                    Description = "Full depth back squat",
                    MuscleGroup = "Legs"
                };

                var pullUp = new Exercise
                {
                    Name = "Pull-Up",
                    Description = "Bodyweight overhand grip pull-up",
                    MuscleGroup = "Back"
                };

                var shoulderPress = new Exercise
                {
                    Name = "Overhead Press",
                    Description = "Standing barbell overhead press",
                    MuscleGroup = "Shoulders"
                };

                context.Exercises.AddRange(benchPress, squat, pullUp, shoulderPress);
                await context.SaveChangesAsync();

                // 4. Seed Sample Workout Plan
                var workoutPlan = new WorkoutPlan
                {
                    Name = "Full Body Foundation",
                    Description = "Core compound movements for full body strength",
                    TrainerId = trainer.Id,
                    CreatedAt = DateTime.UtcNow,
                    WorkoutExercises = new List<WorkoutExercise>
                    {
                        new() { ExerciseId = benchPress.Id, Sets = 4, Repetitions = 8 },
                        new() { ExerciseId = squat.Id, Sets = 4, Repetitions = 6 },
                        new() { ExerciseId = pullUp.Id, Sets = 3, Repetitions = 10 }
                    }
                };

                context.WorkoutPlans.Add(workoutPlan);

                // 5. Seed Sample Active Membership
                var membership = new Membership
                {
                    UserId = regularUser.Id,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(20),
                    Type = MembershipType.Monthly,
                    Status = MembershipStatus.Active
                };

                context.Memberships.Add(membership);

                await context.SaveChangesAsync();
                logger.LogInformation("Database seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}

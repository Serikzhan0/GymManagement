using GymManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<Membership> Memberships => Set<Membership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email for User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role).HasConversion<string>();
        });

        // Composite key for WorkoutExercise
        modelBuilder.Entity<WorkoutExercise>(entity =>
        {
            entity.HasKey(we => new { we.WorkoutPlanId, we.ExerciseId });

            entity.HasOne(we => we.WorkoutPlan)
                .WithMany(wp => wp.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutPlanId);

            entity.HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId);
        });

        // Membership enum conversions
        modelBuilder.Entity<Membership>(entity =>
        {
            entity.Property(m => m.Type).HasConversion<string>();
            entity.Property(m => m.Status).HasConversion<string>();
        });
    }
}

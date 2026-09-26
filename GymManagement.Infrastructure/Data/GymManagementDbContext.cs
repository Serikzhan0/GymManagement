using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Data;

public class GymManagementDbContext : DbContext
{
    public GymManagementDbContext(DbContextOptions<GymManagementDbContext> options) : base(options)
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

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(u => u.CreatedAt).IsRequired();

            entity.HasMany(u => u.Memberships)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Trainer
        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Specialization).IsRequired().HasMaxLength(100);
            entity.Property(t => t.ExperienceYears).IsRequired();

            entity.HasMany(t => t.WorkoutPlans)
                .WithOne(wp => wp.Trainer)
                .HasForeignKey(wp => wp.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Exercise
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.MuscleGroup).IsRequired().HasMaxLength(50);
        });

        // WorkoutPlan
        modelBuilder.Entity<WorkoutPlan>(entity =>
        {
            entity.HasKey(wp => wp.Id);
            entity.Property(wp => wp.Name).IsRequired().HasMaxLength(100);
            entity.Property(wp => wp.Description).IsRequired().HasMaxLength(500);
            entity.Property(wp => wp.TrainerId).IsRequired();
            entity.Property(wp => wp.CreatedAt).IsRequired();

            entity.HasMany(wp => wp.WorkoutExercises)
                .WithOne(we => we.WorkoutPlan)
                .HasForeignKey(we => we.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // WorkoutExercise (Many-to-Many Join Entity)
        modelBuilder.Entity<WorkoutExercise>(entity =>
        {
            entity.HasKey(we => new { we.WorkoutPlanId, we.ExerciseId });
            entity.Property(we => we.Sets).IsRequired();
            entity.Property(we => we.Repetitions).IsRequired();

            entity.HasOne(we => we.WorkoutPlan)
                .WithMany(wp => wp.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Membership
        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.UserId).IsRequired();
            entity.Property(m => m.StartDate).IsRequired();
            entity.Property(m => m.EndDate).IsRequired();
            entity.Property(m => m.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(m => m.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

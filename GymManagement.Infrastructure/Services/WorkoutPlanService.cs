using GymManagement.Application.Common.Exceptions;
using GymManagement.Application.DTOs.WorkoutPlans;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Services;

public class WorkoutPlanService : IWorkoutPlanService
{
    private readonly GymManagementDbContext _dbContext;

    public WorkoutPlanService(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<WorkoutPlanDto>> GetAllAsync()
    {
        return await _dbContext.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .Select(wp => new WorkoutPlanDto
            {
                Id = wp.Id,
                Name = wp.Name,
                Description = wp.Description,
                TrainerId = wp.TrainerId,
                TrainerName = wp.Trainer != null ? wp.Trainer.Name : string.Empty,
                CreatedAt = wp.CreatedAt,
                Exercises = wp.WorkoutExercises.Select(we => new WorkoutExerciseItemDto
                {
                    ExerciseId = we.ExerciseId,
                    ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                    Sets = we.Sets,
                    Repetitions = we.Repetitions
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<WorkoutPlanDto> GetByIdAsync(int id)
    {
        var wp = await _dbContext.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(wp => wp.Id == id);

        if (wp == null)
        {
            throw new NotFoundException("WorkoutPlan", id);
        }

        return new WorkoutPlanDto
        {
            Id = wp.Id,
            Name = wp.Name,
            Description = wp.Description,
            TrainerId = wp.TrainerId,
            TrainerName = wp.Trainer != null ? wp.Trainer.Name : string.Empty,
            CreatedAt = wp.CreatedAt,
            Exercises = wp.WorkoutExercises.Select(we => new WorkoutExerciseItemDto
            {
                ExerciseId = we.ExerciseId,
                ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                Sets = we.Sets,
                Repetitions = we.Repetitions
            }).ToList()
        };
    }

    public async Task<WorkoutPlanDto> CreateAsync(CreateWorkoutPlanDto dto, int currentUserId, string currentUserRole)
    {
        if (dto.Exercises == null || dto.Exercises.Count == 0)
        {
            throw new BadRequestException("A workout plan must contain at least one exercise.");
        }

        var distinctExerciseIds = dto.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
        if (distinctExerciseIds.Count != dto.Exercises.Count)
        {
            throw new BadRequestException("The same exercise cannot be added twice to the same workout plan.");
        }

        var trainerExists = await _dbContext.Trainers.AnyAsync(t => t.Id == dto.TrainerId);
        if (!trainerExists)
        {
            throw new NotFoundException("Trainer", dto.TrainerId);
        }

        var existingExerciseIds = await _dbContext.Exercises
            .Where(e => distinctExerciseIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        var missingExerciseIds = distinctExerciseIds.Except(existingExerciseIds).ToList();
        if (missingExerciseIds.Any())
        {
            throw new NotFoundException($"Exercises with IDs [{string.Join(", ", missingExerciseIds)}] were not found.");
        }

        var workoutPlan = new WorkoutPlan
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            TrainerId = dto.TrainerId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in dto.Exercises)
        {
            workoutPlan.WorkoutExercises.Add(new WorkoutExercise
            {
                ExerciseId = item.ExerciseId,
                Sets = item.Sets,
                Repetitions = item.Repetitions
            });
        }

        _dbContext.WorkoutPlans.Add(workoutPlan);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(workoutPlan.Id);
    }

    public async Task<WorkoutPlanDto> UpdateAsync(int id, UpdateWorkoutPlanDto dto, int currentUserId, string currentUserRole)
    {
        if (dto.Exercises == null || dto.Exercises.Count == 0)
        {
            throw new BadRequestException("A workout plan must contain at least one exercise.");
        }

        var distinctExerciseIds = dto.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
        if (distinctExerciseIds.Count != dto.Exercises.Count)
        {
            throw new BadRequestException("The same exercise cannot be added twice to the same workout plan.");
        }

        var workoutPlan = await _dbContext.WorkoutPlans
            .Include(wp => wp.WorkoutExercises)
            .FirstOrDefaultAsync(wp => wp.Id == id);

        if (workoutPlan == null)
        {
            throw new NotFoundException("WorkoutPlan", id);
        }

        var existingExerciseIds = await _dbContext.Exercises
            .Where(e => distinctExerciseIds.Contains(e.Id))
            .Select(e => e.Id)
            .ToListAsync();

        var missingExerciseIds = distinctExerciseIds.Except(existingExerciseIds).ToList();
        if (missingExerciseIds.Any())
        {
            throw new NotFoundException($"Exercises with IDs [{string.Join(", ", missingExerciseIds)}] were not found.");
        }

        workoutPlan.Name = dto.Name.Trim();
        workoutPlan.Description = dto.Description.Trim();

        workoutPlan.WorkoutExercises.Clear();
        foreach (var item in dto.Exercises)
        {
            workoutPlan.WorkoutExercises.Add(new WorkoutExercise
            {
                WorkoutPlanId = workoutPlan.Id,
                ExerciseId = item.ExerciseId,
                Sets = item.Sets,
                Repetitions = item.Repetitions
            });
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(workoutPlan.Id);
    }

    public async Task DeleteAsync(int id, int currentUserId, string currentUserRole)
    {
        var workoutPlan = await _dbContext.WorkoutPlans.FindAsync(id);
        if (workoutPlan == null)
        {
            throw new NotFoundException("WorkoutPlan", id);
        }

        _dbContext.WorkoutPlans.Remove(workoutPlan);
        await _dbContext.SaveChangesAsync();
    }
}

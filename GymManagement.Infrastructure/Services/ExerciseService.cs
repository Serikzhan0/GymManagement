using GymManagement.Application.Common.Exceptions;
using GymManagement.Application.DTOs.Exercises;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Services;

public class ExerciseService : IExerciseService
{
    private readonly GymManagementDbContext _dbContext;

    public ExerciseService(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ExerciseDto>> GetAllAsync(string? muscleGroup, string? search)
    {
        var query = _dbContext.Exercises.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(muscleGroup))
        {
            var mg = muscleGroup.Trim().ToLower();
            query = query.Where(e => e.MuscleGroup.ToLower() == mg);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(s) || e.Description.ToLower().Contains(s));
        }

        return await query
            .Select(e => new ExerciseDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                MuscleGroup = e.MuscleGroup
            })
            .ToListAsync();
    }

    public async Task<ExerciseDto> GetByIdAsync(int id)
    {
        var exercise = await _dbContext.Exercises
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null)
        {
            throw new NotFoundException("Exercise", id);
        }

        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            MuscleGroup = exercise.MuscleGroup
        };
    }

    public async Task<ExerciseDto> CreateAsync(CreateExerciseDto dto)
    {
        var exercise = new Exercise
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            MuscleGroup = dto.MuscleGroup.Trim()
        };

        _dbContext.Exercises.Add(exercise);
        await _dbContext.SaveChangesAsync();

        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            MuscleGroup = exercise.MuscleGroup
        };
    }

    public async Task<ExerciseDto> UpdateAsync(int id, UpdateExerciseDto dto)
    {
        var exercise = await _dbContext.Exercises.FindAsync(id);

        if (exercise == null)
        {
            throw new NotFoundException("Exercise", id);
        }

        exercise.Name = dto.Name.Trim();
        exercise.Description = dto.Description.Trim();
        exercise.MuscleGroup = dto.MuscleGroup.Trim();

        await _dbContext.SaveChangesAsync();

        return new ExerciseDto
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            MuscleGroup = exercise.MuscleGroup
        };
    }

    public async Task DeleteAsync(int id)
    {
        var exercise = await _dbContext.Exercises
            .Include(e => e.WorkoutExercises)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exercise == null)
        {
            throw new NotFoundException("Exercise", id);
        }

        if (exercise.WorkoutExercises.Any())
        {
            throw new BadRequestException("Cannot delete exercise that is currently part of workout plans.");
        }

        _dbContext.Exercises.Remove(exercise);
        await _dbContext.SaveChangesAsync();
    }
}

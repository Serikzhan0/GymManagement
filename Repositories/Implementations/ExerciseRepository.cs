using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Repositories.Implementations;

public class ExerciseRepository : IExerciseRepository
{
    private readonly AppDbContext _context;

    public ExerciseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Exercise>> GetAllAsync(string? muscleGroup, string? search)
    {
        var query = _context.Exercises.AsNoTracking().AsQueryable();

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

        return await query.ToListAsync();
    }

    public async Task<Exercise?> GetByIdAsync(int id)
    {
        return await _context.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Exercise?> GetByIdTrackingAsync(int id)
    {
        return await _context.Exercises.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Exercise?> GetByIdWithWorkoutExercisesAsync(int id)
    {
        return await _context.Exercises
            .Include(e => e.WorkoutExercises)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<int> CountByIdsAsync(IEnumerable<int> ids)
    {
        var idList = ids.Distinct().ToList();
        return await _context.Exercises.CountAsync(e => idList.Contains(e.Id));
    }

    public async Task AddAsync(Exercise exercise)
    {
        await _context.Exercises.AddAsync(exercise);
    }

    public Task UpdateAsync(Exercise exercise)
    {
        _context.Exercises.Update(exercise);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Exercise exercise)
    {
        _context.Exercises.Remove(exercise);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Exercises.AnyAsync(e => e.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Exercises.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

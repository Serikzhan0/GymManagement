using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Repositories.Implementations;

public class WorkoutPlanRepository : IWorkoutPlanRepository
{
    private readonly AppDbContext _context;

    public WorkoutPlanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WorkoutPlan>> GetAllAsync()
    {
        return await _context.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .ToListAsync();
    }

    public async Task<WorkoutPlan?> GetByIdAsync(int id)
    {
        return await _context.WorkoutPlans
            .AsNoTracking()
            .Include(wp => wp.Trainer)
            .Include(wp => wp.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(wp => wp.Id == id);
    }

    public async Task<WorkoutPlan?> GetByIdForUpdateAsync(int id)
    {
        return await _context.WorkoutPlans
            .Include(wp => wp.WorkoutExercises)
            .FirstOrDefaultAsync(wp => wp.Id == id);
    }

    public async Task AddAsync(WorkoutPlan plan)
    {
        await _context.WorkoutPlans.AddAsync(plan);
    }

    public Task UpdateAsync(WorkoutPlan plan)
    {
        _context.WorkoutPlans.Update(plan);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(WorkoutPlan plan)
    {
        _context.WorkoutPlans.Remove(plan);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync()
    {
        return await _context.WorkoutPlans.CountAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

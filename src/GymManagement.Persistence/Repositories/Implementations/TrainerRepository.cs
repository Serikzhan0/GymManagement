using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Repositories.Implementations;

public class TrainerRepository : ITrainerRepository
{
    private readonly AppDbContext _context;

    public TrainerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Trainer>> GetAllAsync()
    {
        return await _context.Trainers.AsNoTracking().ToListAsync();
    }

    public async Task<Trainer?> GetByIdAsync(int id)
    {
        return await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Trainer?> GetByIdWithPlansAsync(int id)
    {
        return await _context.Trainers
            .Include(t => t.WorkoutPlans)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(Trainer trainer)
    {
        await _context.Trainers.AddAsync(trainer);
    }

    public Task UpdateAsync(Trainer trainer)
    {
        _context.Trainers.Update(trainer);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Trainer trainer)
    {
        _context.Trainers.Remove(trainer);
        return Task.CompletedTask;
    }

    public async Task<bool> HasWorkoutPlansAsync(int trainerId)
    {
        return await _context.WorkoutPlans.AnyAsync(w => w.TrainerId == trainerId);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Trainers.CountAsync();
    }

    public async Task<(string TrainerName, int PlansCount)?> GetTopTrainerByWorkoutPlansAsync()
    {
        var top = await _context.WorkoutPlans
            .GroupBy(w => w.TrainerId)
            .Select(g => new { TrainerId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        if (top == null) return null;

        var trainer = await _context.Trainers.FindAsync(top.TrainerId);
        return trainer != null ? (trainer.Name, top.Count) : null;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

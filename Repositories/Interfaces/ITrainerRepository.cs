using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface ITrainerRepository
{
    Task<IEnumerable<Trainer>> GetAllAsync();
    Task<Trainer?> GetByIdAsync(int id);
    Task<Trainer?> GetByIdWithPlansAsync(int id);
    Task AddAsync(Trainer trainer);
    Task UpdateAsync(Trainer trainer);
    Task DeleteAsync(Trainer trainer);
    Task<bool> HasWorkoutPlansAsync(int trainerId);
    Task<int> CountAsync();
    Task<(string TrainerName, int PlansCount)?> GetTopTrainerByWorkoutPlansAsync();
    Task SaveChangesAsync();
}

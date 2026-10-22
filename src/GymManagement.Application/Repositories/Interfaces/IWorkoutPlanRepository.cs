using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface IWorkoutPlanRepository
{
    Task<IEnumerable<WorkoutPlan>> GetAllAsync();
    Task<WorkoutPlan?> GetByIdAsync(int id);
    Task<WorkoutPlan?> GetByIdForUpdateAsync(int id);
    Task AddAsync(WorkoutPlan plan);
    Task UpdateAsync(WorkoutPlan plan);
    Task DeleteAsync(WorkoutPlan plan);
    Task<int> CountAsync();
    Task SaveChangesAsync();
}

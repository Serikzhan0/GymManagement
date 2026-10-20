using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface IExerciseRepository
{
    Task<IEnumerable<Exercise>> GetAllAsync(string? muscleGroup, string? search);
    Task<Exercise?> GetByIdAsync(int id);
    Task<Exercise?> GetByIdTrackingAsync(int id);
    Task<Exercise?> GetByIdWithWorkoutExercisesAsync(int id);
    Task<int> CountByIdsAsync(IEnumerable<int> ids);
    Task AddAsync(Exercise exercise);
    Task UpdateAsync(Exercise exercise);
    Task DeleteAsync(Exercise exercise);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task SaveChangesAsync();
}

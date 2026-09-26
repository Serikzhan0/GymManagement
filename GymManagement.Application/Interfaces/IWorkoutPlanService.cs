using GymManagement.Application.DTOs.WorkoutPlans;

namespace GymManagement.Application.Interfaces;

public interface IWorkoutPlanService
{
    Task<IEnumerable<WorkoutPlanDto>> GetAllAsync();
    Task<WorkoutPlanDto> GetByIdAsync(int id);
    Task<WorkoutPlanDto> CreateAsync(CreateWorkoutPlanDto dto, int currentUserId, string currentUserRole);
    Task<WorkoutPlanDto> UpdateAsync(int id, UpdateWorkoutPlanDto dto, int currentUserId, string currentUserRole);
    Task DeleteAsync(int id, int currentUserId, string currentUserRole);
}

using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface IWorkoutPlanService
{
    Task<IEnumerable<WorkoutPlanDto>> GetAllAsync();
    Task<ServiceResult<WorkoutPlanDto>> GetByIdAsync(int id);
    Task<ServiceResult<WorkoutPlanDto>> CreateAsync(CreateWorkoutPlanDto dto);
    Task<ServiceResult<WorkoutPlanDto>> UpdateAsync(int id, UpdateWorkoutPlanDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}

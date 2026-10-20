using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface IExerciseService
{
    Task<IEnumerable<ExerciseDto>> GetAllAsync(string? muscleGroup, string? search);
    Task<ServiceResult<ExerciseDto>> GetByIdAsync(int id);
    Task<ServiceResult<ExerciseDto>> CreateAsync(CreateExerciseDto dto);
    Task<ServiceResult<ExerciseDto>> UpdateAsync(int id, UpdateExerciseDto dto);
    Task<ServiceResult<ExerciseDto>> UpdateMediaAsync(int id, UpdateExerciseMediaDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}

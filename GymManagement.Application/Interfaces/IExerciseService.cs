using GymManagement.Application.DTOs.Exercises;

namespace GymManagement.Application.Interfaces;

public interface IExerciseService
{
    Task<IEnumerable<ExerciseDto>> GetAllAsync(string? muscleGroup, string? search);
    Task<ExerciseDto> GetByIdAsync(int id);
    Task<ExerciseDto> CreateAsync(CreateExerciseDto dto);
    Task<ExerciseDto> UpdateAsync(int id, UpdateExerciseDto dto);
    Task DeleteAsync(int id);
}

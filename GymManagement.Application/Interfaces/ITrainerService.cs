using GymManagement.Application.DTOs.Trainers;

namespace GymManagement.Application.Interfaces;

public interface ITrainerService
{
    Task<IEnumerable<TrainerDto>> GetAllAsync();
    Task<TrainerDto> GetByIdAsync(int id);
    Task<TrainerDto> CreateAsync(CreateTrainerDto dto);
    Task<TrainerDto> UpdateAsync(int id, UpdateTrainerDto dto);
    Task DeleteAsync(int id);
}

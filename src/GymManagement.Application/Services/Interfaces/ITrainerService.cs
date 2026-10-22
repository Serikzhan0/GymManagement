using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface ITrainerService
{
    Task<IEnumerable<TrainerDto>> GetAllAsync();
    Task<ServiceResult<TrainerDto>> GetByIdAsync(int id);
    Task<ServiceResult<TrainerDto>> CreateAsync(CreateTrainerDto dto);
    Task<ServiceResult<TrainerDto>> UpdateAsync(int id, UpdateTrainerDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}

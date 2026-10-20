using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface ICheckInService
{
    Task<ServiceResult<CheckInDto>> ProcessCheckInAsync(CreateCheckInDto dto, int currentUserId, string currentUserRole);
    Task<IEnumerable<CheckInDto>> GetHistoryAsync(int currentUserId, string currentUserRole);
}

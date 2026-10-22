using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface ICheckInRepository
{
    Task<IEnumerable<CheckIn>> GetHistoryAsync(int? userId, int limit = 50);
    Task AddAsync(CheckIn checkIn);
    Task<int> CountTodayGrantedAsync(DateTime startOfToday);
    Task SaveChangesAsync();
}

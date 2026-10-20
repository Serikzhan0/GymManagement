using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface IMembershipRepository
{
    Task<IEnumerable<Membership>> GetAllAsync(int? userId);
    Task<Membership?> GetByIdAsync(int id);
    Task<Membership?> GetByIdWithUserAsync(int id);
    Task<Membership?> GetActiveByUserIdAsync(int userId, DateTime now);
    Task<Membership?> GetFrozenByUserIdAsync(int userId);
    Task<List<Membership>> GetActiveListByUserIdAsync(int userId, DateTime now);
    Task AutoExpireAsync(int? userId, DateTime now);
    Task<bool> HasOtherActiveMembershipAsync(int userId, int excludeId, DateTime now);
    Task AddAsync(Membership membership);
    Task UpdateAsync(Membership membership);
    Task DeleteAsync(Membership membership);
    Task<int> CountActiveAsync(DateTime now);
    Task<int> CountExpiredAsync(DateTime now);
    Task SaveChangesAsync();
}

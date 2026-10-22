using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<int> CountByRoleAsync(UserRole role);
    Task SaveChangesAsync();
}

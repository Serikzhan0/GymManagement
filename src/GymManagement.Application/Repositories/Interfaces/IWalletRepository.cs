using GymManagementApi.Models;

namespace GymManagementApi.Repositories.Interfaces;

public interface IWalletRepository
{
    Task<IEnumerable<WalletTransaction>> GetTransactionsByUserIdAsync(int userId, int limit = 30);
    Task AddTransactionAsync(WalletTransaction transaction);
    Task SaveChangesAsync();
}

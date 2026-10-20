using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Repositories.Implementations;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WalletTransaction>> GetTransactionsByUserIdAsync(int userId, int limit = 30)
    {
        return await _context.WalletTransactions
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddTransactionAsync(WalletTransaction transaction)
    {
        await _context.WalletTransactions.AddAsync(transaction);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

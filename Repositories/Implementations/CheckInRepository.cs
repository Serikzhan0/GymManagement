using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Repositories.Implementations;

public class CheckInRepository : ICheckInRepository
{
    private readonly AppDbContext _context;

    public CheckInRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CheckIn>> GetHistoryAsync(int? userId, int limit = 50)
    {
        var query = _context.CheckIns
            .AsNoTracking()
            .Include(c => c.User)
            .OrderByDescending(c => c.CheckInTime)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(c => c.UserId == userId.Value);
        }

        return await query.Take(limit).ToListAsync();
    }

    public async Task AddAsync(CheckIn checkIn)
    {
        await _context.CheckIns.AddAsync(checkIn);
    }

    public async Task<int> CountTodayGrantedAsync(DateTime startOfToday)
    {
        return await _context.CheckIns.CountAsync(c => c.CheckInTime >= startOfToday && c.Status == "Granted");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

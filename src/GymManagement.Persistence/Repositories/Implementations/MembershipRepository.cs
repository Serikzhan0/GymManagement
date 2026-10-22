using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Repositories.Implementations;

public class MembershipRepository : IMembershipRepository
{
    private readonly AppDbContext _context;

    public MembershipRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Membership>> GetAllAsync(int? userId)
    {
        var query = _context.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(m => m.UserId == userId.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<Membership?> GetByIdAsync(int id)
    {
        return await _context.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Membership?> GetByIdWithUserAsync(int id)
    {
        return await _context.Memberships
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Membership?> GetActiveByUserIdAsync(int userId, DateTime now)
    {
        return await _context.Memberships
            .Where(m => m.UserId == userId &&
                        m.Status == MembershipStatus.Active &&
                        m.EndDate >= now)
            .OrderByDescending(m => m.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task<Membership?> GetFrozenByUserIdAsync(int userId)
    {
        return await _context.Memberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.Status == MembershipStatus.Frozen);
    }

    public async Task<List<Membership>> GetActiveListByUserIdAsync(int userId, DateTime now)
    {
        return await _context.Memberships
            .Where(m => m.UserId == userId &&
                        m.Status == MembershipStatus.Active &&
                        m.EndDate >= now)
            .ToListAsync();
    }

    public async Task AutoExpireAsync(int? userId, DateTime now)
    {
        var query = _context.Memberships
            .Where(m => m.Status == MembershipStatus.Active && m.EndDate < now);

        if (userId.HasValue)
        {
            query = query.Where(m => m.UserId == userId.Value);
        }

        var expired = await query.ToListAsync();
        if (expired.Any())
        {
            foreach (var m in expired)
            {
                m.Status = MembershipStatus.Expired;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasOtherActiveMembershipAsync(int userId, int excludeId, DateTime now)
    {
        return await _context.Memberships
            .AnyAsync(other => other.Id != excludeId &&
                               other.UserId == userId &&
                               other.Status == MembershipStatus.Active &&
                               other.EndDate >= now);
    }

    public async Task AddAsync(Membership membership)
    {
        await _context.Memberships.AddAsync(membership);
    }

    public Task UpdateAsync(Membership membership)
    {
        _context.Memberships.Update(membership);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Membership membership)
    {
        _context.Memberships.Remove(membership);
        return Task.CompletedTask;
    }

    public async Task<int> CountActiveAsync(DateTime now)
    {
        return await _context.Memberships.CountAsync(m => m.Status == MembershipStatus.Active && m.EndDate >= now);
    }

    public async Task<int> CountExpiredAsync(DateTime now)
    {
        return await _context.Memberships.CountAsync(m => m.Status == MembershipStatus.Expired || m.EndDate < now);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

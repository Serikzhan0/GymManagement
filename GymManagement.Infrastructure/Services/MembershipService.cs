using GymManagement.Application.Common.Exceptions;
using GymManagement.Application.DTOs.Memberships;
using GymManagement.Application.Interfaces;
using GymManagement.Domain.Entities;
using GymManagement.Domain.Enums;
using GymManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Infrastructure.Services;

public class MembershipService : IMembershipService
{
    private readonly GymManagementDbContext _dbContext;

    public MembershipService(GymManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task AutoExpireMembershipsAsync(int? userId = null)
    {
        var now = DateTime.UtcNow;
        var query = _dbContext.Memberships
            .Where(m => m.Status == MembershipStatus.Active && m.EndDate < now);

        if (userId.HasValue)
        {
            query = query.Where(m => m.UserId == userId.Value);
        }

        var expiredMemberships = await query.ToListAsync();
        if (expiredMemberships.Any())
        {
            foreach (var m in expiredMemberships)
            {
                m.Status = MembershipStatus.Expired;
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MembershipDto>> GetAllAsync(int currentUserId, string currentUserRole)
    {
        await AutoExpireMembershipsAsync(currentUserRole == nameof(UserRole.Admin) ? null : currentUserId);

        var query = _dbContext.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .AsQueryable();

        if (currentUserRole != nameof(UserRole.Admin))
        {
            query = query.Where(m => m.UserId == currentUserId);
        }

        return await query
            .Select(m => new MembershipDto
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User != null ? m.User.Name : string.Empty,
                UserEmail = m.User != null ? m.User.Email : string.Empty,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Type = m.Type.ToString(),
                Status = m.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task<MembershipDto> GetByIdAsync(int id, int currentUserId, string currentUserRole)
    {
        await AutoExpireMembershipsAsync();

        var m = await _dbContext.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (m == null)
        {
            throw new NotFoundException("Membership", id);
        }

        if (currentUserRole != nameof(UserRole.Admin) && m.UserId != currentUserId)
        {
            throw new ForbiddenException("You do not have permission to view this membership.");
        }

        return new MembershipDto
        {
            Id = m.Id,
            UserId = m.UserId,
            UserName = m.User != null ? m.User.Name : string.Empty,
            UserEmail = m.User != null ? m.User.Email : string.Empty,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Type = m.Type.ToString(),
            Status = m.Status.ToString()
        };
    }

    public async Task<MembershipDto> CreateAsync(CreateMembershipDto dto, int currentUserId, string currentUserRole)
    {
        // Business Rule 9: A user cannot create a membership for another user
        if (currentUserRole != nameof(UserRole.Admin) && dto.UserId != currentUserId)
        {
            throw new ForbiddenException("You cannot create a membership for another user.");
        }

        var user = await _dbContext.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            throw new NotFoundException("User", dto.UserId);
        }

        // Business Rule 2: Ensure any past memberships are expired first
        await AutoExpireMembershipsAsync(dto.UserId);

        var now = DateTime.UtcNow;

        // Business Rule 1: A user cannot have two active memberships at the same time
        var hasActiveMembership = await _dbContext.Memberships
            .AnyAsync(m => m.UserId == dto.UserId &&
                           m.Status == MembershipStatus.Active &&
                           m.EndDate >= now);

        if (hasActiveMembership)
        {
            throw new BadRequestException("User already has an active membership");
        }

        var initialStatus = dto.EndDate < now ? MembershipStatus.Expired : MembershipStatus.Active;

        var membership = new Membership
        {
            UserId = dto.UserId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Type = dto.Type,
            Status = initialStatus
        };

        _dbContext.Memberships.Add(membership);
        await _dbContext.SaveChangesAsync();

        return new MembershipDto
        {
            Id = membership.Id,
            UserId = membership.UserId,
            UserName = user.Name,
            UserEmail = user.Email,
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            Type = membership.Type.ToString(),
            Status = membership.Status.ToString()
        };
    }

    public async Task<MembershipDto> UpdateAsync(int id, UpdateMembershipDto dto, int currentUserId, string currentUserRole)
    {
        await AutoExpireMembershipsAsync();

        var membership = await _dbContext.Memberships
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membership == null)
        {
            throw new NotFoundException("Membership", id);
        }

        if (currentUserRole != nameof(UserRole.Admin) && membership.UserId != currentUserId)
        {
            throw new ForbiddenException("You do not have permission to modify this membership.");
        }

        var now = DateTime.UtcNow;
        var targetStatus = dto.Status;

        if (dto.EndDate < now && targetStatus == MembershipStatus.Active)
        {
            targetStatus = MembershipStatus.Expired;
        }

        // Business Rule 1: Check active membership conflict if updating to Active
        if (targetStatus == MembershipStatus.Active)
        {
            var hasOtherActive = await _dbContext.Memberships
                .AnyAsync(m => m.Id != id &&
                               m.UserId == membership.UserId &&
                               m.Status == MembershipStatus.Active &&
                               m.EndDate >= now);

            if (hasOtherActive)
            {
                throw new BadRequestException("User already has an active membership");
            }
        }

        membership.StartDate = dto.StartDate;
        membership.EndDate = dto.EndDate;
        membership.Type = dto.Type;
        membership.Status = targetStatus;

        await _dbContext.SaveChangesAsync();

        return new MembershipDto
        {
            Id = membership.Id,
            UserId = membership.UserId,
            UserName = membership.User?.Name ?? string.Empty,
            UserEmail = membership.User?.Email ?? string.Empty,
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            Type = membership.Type.ToString(),
            Status = membership.Status.ToString()
        };
    }

    public async Task DeleteAsync(int id, int currentUserId, string currentUserRole)
    {
        var membership = await _dbContext.Memberships.FindAsync(id);
        if (membership == null)
        {
            throw new NotFoundException("Membership", id);
        }

        if (currentUserRole != nameof(UserRole.Admin) && membership.UserId != currentUserId)
        {
            throw new ForbiddenException("You do not have permission to delete this membership.");
        }

        _dbContext.Memberships.Remove(membership);
        await _dbContext.SaveChangesAsync();
    }
}

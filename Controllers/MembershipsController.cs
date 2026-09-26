using System.Security.Claims;
using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementApi.Controllers;

[ApiController]
[Route("api/memberships")]
public class MembershipsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<CreateMembershipDto> _createValidator;
    private readonly IValidator<UpdateMembershipDto> _updateValidator;

    public MembershipsController(
        AppDbContext context,
        IValidator<CreateMembershipDto> createValidator,
        IValidator<UpdateMembershipDto> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    private int CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    // Helper: automatically expire past memberships
    private async Task AutoExpireMembershipsAsync(int? userId = null)
    {
        var now = DateTime.UtcNow;
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

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MembershipDto>>> GetAll()
    {
        await AutoExpireMembershipsAsync(CurrentUserRole == nameof(UserRole.Admin) ? null : CurrentUserId);

        var query = _context.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .AsQueryable();

        if (CurrentUserRole != nameof(UserRole.Admin))
        {
            query = query.Where(m => m.UserId == CurrentUserId);
        }

        var list = await query
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

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> GetById(int id)
    {
        await AutoExpireMembershipsAsync();

        var m = await _context.Memberships
            .AsNoTracking()
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (m == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Membership with ID '{id}' was not found."));
        }

        if (CurrentUserRole != nameof(UserRole.Admin) && m.UserId != CurrentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(StatusCodes.Status403Forbidden, "You do not have permission to view this membership."));
        }

        return Ok(new MembershipDto
        {
            Id = m.Id,
            UserId = m.UserId,
            UserName = m.User != null ? m.User.Name : string.Empty,
            UserEmail = m.User != null ? m.User.Email : string.Empty,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Type = m.Type.ToString(),
            Status = m.Status.ToString()
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Create([FromBody] CreateMembershipDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        // Rule 10: User can only create membership for themselves
        if (CurrentUserRole != nameof(UserRole.Admin) && dto.UserId != CurrentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(StatusCodes.Status403Forbidden, "You cannot create a membership for another user."));
        }

        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"User with ID '{dto.UserId}' was not found."));
        }

        // Rule 3: auto-expire past memberships
        await AutoExpireMembershipsAsync(dto.UserId);

        var now = DateTime.UtcNow;

        // Rule 2: User cannot have two active memberships at the same time
        var hasActive = await _context.Memberships
            .AnyAsync(m => m.UserId == dto.UserId &&
                           m.Status == MembershipStatus.Active &&
                           m.EndDate >= now);

        if (hasActive)
        {
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, "User already has an active membership"));
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

        _context.Memberships.Add(membership);
        await _context.SaveChangesAsync();

        var result = new MembershipDto
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

        return CreatedAtAction(nameof(GetById), new { id = membership.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<MembershipDto>> Update(int id, [FromBody] UpdateMembershipDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, firstError, validation.ToDictionary()));
        }

        await AutoExpireMembershipsAsync();

        var m = await _context.Memberships
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (m == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Membership with ID '{id}' was not found."));
        }

        if (CurrentUserRole != nameof(UserRole.Admin) && m.UserId != CurrentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(StatusCodes.Status403Forbidden, "You do not have permission to modify this membership."));
        }

        var now = DateTime.UtcNow;
        var targetStatus = dto.Status;
        if (dto.EndDate < now && targetStatus == MembershipStatus.Active)
        {
            targetStatus = MembershipStatus.Expired;
        }

        // Rule 2 check if updating to Active
        if (targetStatus == MembershipStatus.Active)
        {
            var hasOtherActive = await _context.Memberships
                .AnyAsync(other => other.Id != id &&
                                   other.UserId == m.UserId &&
                                   other.Status == MembershipStatus.Active &&
                                   other.EndDate >= now);

            if (hasOtherActive)
            {
                return BadRequest(new ErrorResponse(StatusCodes.Status400BadRequest, "User already has an active membership"));
            }
        }

        m.StartDate = dto.StartDate;
        m.EndDate = dto.EndDate;
        m.Type = dto.Type;
        m.Status = targetStatus;

        await _context.SaveChangesAsync();

        return Ok(new MembershipDto
        {
            Id = m.Id,
            UserId = m.UserId,
            UserName = m.User?.Name ?? string.Empty,
            UserEmail = m.User?.Email ?? string.Empty,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            Type = m.Type.ToString(),
            Status = m.Status.ToString()
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var m = await _context.Memberships.FindAsync(id);
        if (m == null)
        {
            return NotFound(new ErrorResponse(StatusCodes.Status404NotFound, $"Membership with ID '{id}' was not found."));
        }

        if (CurrentUserRole != nameof(UserRole.Admin) && m.UserId != CurrentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(StatusCodes.Status403Forbidden, "You do not have permission to delete this membership."));
        }

        _context.Memberships.Remove(m);
        await _context.SaveChangesAsync();

        return Ok(new { statusCode = 200, message = "Membership deleted successfully." });
    }
}

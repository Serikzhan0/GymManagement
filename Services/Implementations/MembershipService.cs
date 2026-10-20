using FluentValidation;
using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class MembershipService : IMembershipService
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IValidator<CreateMembershipDto> _createValidator;
    private readonly IValidator<UpdateMembershipDto> _updateValidator;

    public MembershipService(
        IMembershipRepository membershipRepository,
        IUserRepository userRepository,
        IWalletRepository walletRepository,
        IValidator<CreateMembershipDto> createValidator,
        IValidator<UpdateMembershipDto> updateValidator)
    {
        _membershipRepository = membershipRepository;
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task AutoExpireMembershipsAsync(int? userId = null)
    {
        await _membershipRepository.AutoExpireAsync(userId, DateTime.UtcNow);
    }

    public async Task<IEnumerable<MembershipDto>> GetAllAsync(int currentUserId, string currentUserRole)
    {
        var targetUserId = currentUserRole == nameof(UserRole.Admin) ? (int?)null : currentUserId;
        await AutoExpireMembershipsAsync(targetUserId);

        var list = await _membershipRepository.GetAllAsync(targetUserId);
        return list.Select(MapToDto);
    }

    public async Task<ServiceResult<MembershipDto>> GetByIdAsync(int id, int currentUserId, string currentUserRole)
    {
        await AutoExpireMembershipsAsync();

        var m = await _membershipRepository.GetByIdAsync(id);
        if (m == null)
        {
            return ServiceResult<MembershipDto>.Failure($"Membership with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (currentUserRole != nameof(UserRole.Admin) && m.UserId != currentUserId)
        {
            return ServiceResult<MembershipDto>.Failure("You do not have permission to view this membership.", StatusCodes.Status403Forbidden);
        }

        return ServiceResult<MembershipDto>.Success(MapToDto(m));
    }

    public async Task<ServiceResult<MembershipDto>> CreateAsync(CreateMembershipDto dto, int currentUserId, string currentUserRole)
    {
        if (dto.UserId <= 0 && currentUserId > 0)
        {
            dto.UserId = currentUserId;
        }

        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<MembershipDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        if (currentUserRole != nameof(UserRole.Admin) && dto.UserId != currentUserId)
        {
            return ServiceResult<MembershipDto>.Failure("You cannot create a membership for another user.", StatusCodes.Status403Forbidden);
        }

        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
        {
            return ServiceResult<MembershipDto>.Failure($"User with ID '{dto.UserId}' was not found.", StatusCodes.Status404NotFound);
        }

        await AutoExpireMembershipsAsync(dto.UserId);

        var now = DateTime.UtcNow;
        decimal price = dto.Type == MembershipType.Yearly ? 149.0m : dto.Type == MembershipType.Quarterly ? 89.0m : 49.0m;

        if (user.Balance < price)
        {
            return ServiceResult<MembershipDto>.Failure(
                $"Недостаточно средств в кошельке APEX. Баланс: ${user.Balance:F2}, требуется: ${price:F2}. Пожалуйста, пополните кошелек.",
                StatusCodes.Status400BadRequest);
        }

        int monthsToAdd = dto.Type == MembershipType.Yearly ? 12 : dto.Type == MembershipType.Quarterly ? 3 : 1;
        var tierName = dto.Type == MembershipType.Yearly ? "ELITE" : dto.Type == MembershipType.Quarterly ? "APEX PRO" : "STARTER";

        var existingActive = await _membershipRepository.GetActiveByUserIdAsync(dto.UserId, now);

        if (existingActive != null && existingActive.Type == dto.Type)
        {
            existingActive.EndDate = existingActive.EndDate.AddMonths(monthsToAdd);
            user.Balance -= price;

            var txExtend = new WalletTransaction
            {
                UserId = user.Id,
                Amount = -price,
                Description = $"Продление абонемента {tierName} (+{monthsToAdd * 30} дн.)",
                Type = "Purchase",
                CreatedAt = DateTime.UtcNow
            };

            await _walletRepository.AddTransactionAsync(txExtend);
            await _userRepository.UpdateAsync(user);
            await _membershipRepository.UpdateAsync(existingActive);
            await _membershipRepository.SaveChangesAsync();

            return ServiceResult<MembershipDto>.Success(new MembershipDto
            {
                Id = existingActive.Id,
                UserId = existingActive.UserId,
                UserName = user.Name,
                UserEmail = user.Email,
                StartDate = existingActive.StartDate,
                EndDate = existingActive.EndDate,
                Type = existingActive.Type.ToString(),
                Status = existingActive.Status.ToString()
            });
        }

        var previousActiveList = await _membershipRepository.GetActiveListByUserIdAsync(dto.UserId, now);
        foreach (var prev in previousActiveList)
        {
            prev.Status = MembershipStatus.Expired;
            await _membershipRepository.UpdateAsync(prev);
        }

        var startUtc = DateTime.SpecifyKind(dto.StartDate < now ? now : dto.StartDate, DateTimeKind.Utc);
        var endUtc = startUtc.AddMonths(monthsToAdd);
        var initialStatus = endUtc < now ? MembershipStatus.Expired : MembershipStatus.Active;

        user.Balance -= price;
        var txNew = new WalletTransaction
        {
            UserId = user.Id,
            Amount = -price,
            Description = $"Покупка абонемента {tierName}",
            Type = "Purchase",
            CreatedAt = DateTime.UtcNow
        };

        await _walletRepository.AddTransactionAsync(txNew);
        await _userRepository.UpdateAsync(user);

        var membership = new Membership
        {
            UserId = dto.UserId,
            StartDate = startUtc,
            EndDate = endUtc,
            Type = dto.Type,
            Status = initialStatus
        };

        await _membershipRepository.AddAsync(membership);
        await _membershipRepository.SaveChangesAsync();

        return ServiceResult<MembershipDto>.Success(new MembershipDto
        {
            Id = membership.Id,
            UserId = membership.UserId,
            UserName = user.Name,
            UserEmail = user.Email,
            StartDate = membership.StartDate,
            EndDate = membership.EndDate,
            Type = membership.Type.ToString(),
            Status = membership.Status.ToString()
        }, StatusCodes.Status201Created);
    }

    public async Task<ServiceResult<MembershipDto>> UpdateAsync(int id, UpdateMembershipDto dto, int currentUserId, string currentUserRole)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
        {
            var firstError = validation.Errors[0].ErrorMessage;
            return ServiceResult<MembershipDto>.Failure(firstError, StatusCodes.Status400BadRequest, validation.ToDictionary());
        }

        await AutoExpireMembershipsAsync();

        var m = await _membershipRepository.GetByIdWithUserAsync(id);
        if (m == null)
        {
            return ServiceResult<MembershipDto>.Failure($"Membership with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (currentUserRole != nameof(UserRole.Admin) && m.UserId != currentUserId)
        {
            return ServiceResult<MembershipDto>.Failure("You do not have permission to modify this membership.", StatusCodes.Status403Forbidden);
        }

        var now = DateTime.UtcNow;
        var targetStatus = dto.Status;
        if (dto.EndDate < now && targetStatus == MembershipStatus.Active)
        {
            targetStatus = MembershipStatus.Expired;
        }

        if (targetStatus == MembershipStatus.Active)
        {
            var hasOtherActive = await _membershipRepository.HasOtherActiveMembershipAsync(m.UserId, id, now);
            if (hasOtherActive)
            {
                return ServiceResult<MembershipDto>.Failure("User already has an active membership", StatusCodes.Status400BadRequest);
            }
        }

        m.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
        m.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);
        m.Type = dto.Type;
        m.Status = targetStatus;

        await _membershipRepository.UpdateAsync(m);
        await _membershipRepository.SaveChangesAsync();

        return ServiceResult<MembershipDto>.Success(MapToDto(m));
    }

    public async Task<ServiceResult> DeleteAsync(int id, int currentUserId, string currentUserRole)
    {
        var m = await _membershipRepository.GetByIdAsync(id);
        if (m == null)
        {
            return ServiceResult.Failure($"Membership with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (currentUserRole != nameof(UserRole.Admin) && m.UserId != currentUserId)
        {
            return ServiceResult.Failure("You do not have permission to delete this membership.", StatusCodes.Status403Forbidden);
        }

        await _membershipRepository.DeleteAsync(m);
        await _membershipRepository.SaveChangesAsync();

        return ServiceResult.Ok("Membership deleted successfully.");
    }

    public async Task<ServiceResult<MembershipDto>> FreezeAsync(int id, int currentUserId, string currentUserRole)
    {
        var m = await _membershipRepository.GetByIdWithUserAsync(id);
        if (m == null)
        {
            return ServiceResult<MembershipDto>.Failure($"Membership with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (currentUserRole != nameof(UserRole.Admin) && m.UserId != currentUserId)
        {
            return ServiceResult<MembershipDto>.Failure("You do not have permission to freeze this membership.", StatusCodes.Status403Forbidden);
        }

        if (m.Status != MembershipStatus.Active)
        {
            return ServiceResult<MembershipDto>.Failure("Only active memberships can be frozen.", StatusCodes.Status400BadRequest);
        }

        m.Status = MembershipStatus.Frozen;
        m.EndDate = m.EndDate.AddDays(14);

        await _membershipRepository.UpdateAsync(m);
        await _membershipRepository.SaveChangesAsync();

        return ServiceResult<MembershipDto>.Success(MapToDto(m));
    }

    public async Task<ServiceResult<MembershipDto>> UnfreezeAsync(int id, int currentUserId, string currentUserRole)
    {
        var m = await _membershipRepository.GetByIdWithUserAsync(id);
        if (m == null)
        {
            return ServiceResult<MembershipDto>.Failure($"Membership with ID '{id}' was not found.", StatusCodes.Status404NotFound);
        }

        if (currentUserRole != nameof(UserRole.Admin) && m.UserId != currentUserId)
        {
            return ServiceResult<MembershipDto>.Failure("You do not have permission to unfreeze this membership.", StatusCodes.Status403Forbidden);
        }

        if (m.Status != MembershipStatus.Frozen)
        {
            return ServiceResult<MembershipDto>.Failure("Only frozen memberships can be unfrozen.", StatusCodes.Status400BadRequest);
        }

        m.Status = MembershipStatus.Active;

        await _membershipRepository.UpdateAsync(m);
        await _membershipRepository.SaveChangesAsync();

        return ServiceResult<MembershipDto>.Success(MapToDto(m));
    }

    private static MembershipDto MapToDto(Membership m) => new()
    {
        Id = m.Id,
        UserId = m.UserId,
        UserName = m.User?.Name ?? string.Empty,
        UserEmail = m.User?.Email ?? string.Empty,
        StartDate = m.StartDate,
        EndDate = m.EndDate,
        Type = m.Type.ToString(),
        Status = m.Status.ToString()
    };
}

using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services.Implementations;

public class CheckInService : ICheckInService
{
    private readonly ICheckInRepository _checkInRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMembershipRepository _membershipRepository;

    public CheckInService(
        ICheckInRepository checkInRepository,
        IUserRepository userRepository,
        IMembershipRepository membershipRepository)
    {
        _checkInRepository = checkInRepository;
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
    }

    public async Task<ServiceResult<CheckInDto>> ProcessCheckInAsync(CreateCheckInDto dto, int currentUserId, string currentUserRole)
    {
        var targetUserId = (currentUserRole == "Admin" && dto.UserId.HasValue) ? dto.UserId.Value : currentUserId;
        var user = await _userRepository.GetByIdAsync(targetUserId);
        if (user == null)
        {
            return ServiceResult<CheckInDto>.Failure($"User with ID '{targetUserId}' not found.", StatusCodes.Status404NotFound);
        }

        var now = DateTime.UtcNow;

        var frozenMembership = await _membershipRepository.GetFrozenByUserIdAsync(targetUserId);
        if (frozenMembership != null)
        {
            var deniedLog = new CheckIn
            {
                UserId = targetUserId,
                CheckInTime = now,
                Status = "Denied",
                Notes = "Доступ запрещен: абонемент временно заморожен."
            };
            await _checkInRepository.AddAsync(deniedLog);
            await _checkInRepository.SaveChangesAsync();

            return ServiceResult<CheckInDto>.Failure(
                "Доступ в зал закрыт: ваш абонемент заморожен (на 14 дней). Нажмите «РАЗМОРОЗИТЬ» в личном кабинете для входа.",
                StatusCodes.Status400BadRequest);
        }

        var activeMembership = await _membershipRepository.GetActiveByUserIdAsync(targetUserId, now);
        if (activeMembership == null)
        {
            var deniedLog = new CheckIn
            {
                UserId = targetUserId,
                CheckInTime = now,
                Status = "Denied",
                Notes = "Доступ запрещен: нет активного абонемента или срок действия истек."
            };
            await _checkInRepository.AddAsync(deniedLog);
            await _checkInRepository.SaveChangesAsync();

            return ServiceResult<CheckInDto>.Failure(
                "Доступ в зал запрещен: у вас нет активного абонемента или срок его действия истек.",
                StatusCodes.Status400BadRequest);
        }

        var checkIn = new CheckIn
        {
            UserId = targetUserId,
            CheckInTime = now,
            Status = "Granted",
            Notes = $"Успешный вход по абонементу ({activeMembership.Type})"
        };

        await _checkInRepository.AddAsync(checkIn);
        await _checkInRepository.SaveChangesAsync();

        return ServiceResult<CheckInDto>.Success(new CheckInDto
        {
            Id = checkIn.Id,
            UserId = checkIn.UserId,
            UserName = user.Name,
            UserEmail = user.Email,
            CheckInTime = checkIn.CheckInTime,
            Status = checkIn.Status,
            Notes = checkIn.Notes
        });
    }

    public async Task<IEnumerable<CheckInDto>> GetHistoryAsync(int currentUserId, string currentUserRole)
    {
        var targetUserId = currentUserRole == "Admin" ? (int?)null : currentUserId;
        var logs = await _checkInRepository.GetHistoryAsync(targetUserId, 50);

        return logs.Select(c => new CheckInDto
        {
            Id = c.Id,
            UserId = c.UserId,
            UserName = c.User?.Name ?? string.Empty,
            UserEmail = c.User?.Email ?? string.Empty,
            CheckInTime = c.CheckInTime,
            Status = c.Status,
            Notes = c.Notes
        });
    }
}

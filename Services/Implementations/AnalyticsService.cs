using GymManagementApi.DTOs;
using GymManagementApi.Models;
using GymManagementApi.Repositories.Interfaces;
using GymManagementApi.Services.Interfaces;

namespace GymManagementApi.Services.Implementations;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUserRepository _userRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly ICheckInRepository _checkInRepository;
    private readonly IWorkoutPlanRepository _workoutPlanRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ITrainerRepository _trainerRepository;

    public AnalyticsService(
        IUserRepository userRepository,
        IMembershipRepository membershipRepository,
        ICheckInRepository checkInRepository,
        IWorkoutPlanRepository workoutPlanRepository,
        IExerciseRepository exerciseRepository,
        ITrainerRepository trainerRepository)
    {
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
        _checkInRepository = checkInRepository;
        _workoutPlanRepository = workoutPlanRepository;
        _exerciseRepository = exerciseRepository;
        _trainerRepository = trainerRepository;
    }

    public async Task<AnalyticsSummaryDto> GetSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var startOfToday = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);

        var totalMembers = await _userRepository.CountByRoleAsync(UserRole.User);
        var activeMemberships = await _membershipRepository.CountActiveAsync(now);
        var expiredMemberships = await _membershipRepository.CountExpiredAsync(now);
        var todayCheckIns = await _checkInRepository.CountTodayGrantedAsync(startOfToday);
        var totalWorkoutPlans = await _workoutPlanRepository.CountAsync();
        var totalExercises = await _exerciseRepository.CountAsync();

        var topTrainer = await _trainerRepository.GetTopTrainerByWorkoutPlansAsync();

        return new AnalyticsSummaryDto
        {
            TotalMembers = totalMembers,
            ActiveMemberships = activeMemberships,
            ExpiredMemberships = expiredMemberships,
            TodayCheckIns = todayCheckIns,
            TotalWorkoutPlans = totalWorkoutPlans,
            TotalExercises = totalExercises,
            TopTrainerName = topTrainer?.TrainerName ?? "Нет программ",
            TopTrainerPlansCount = topTrainer?.PlansCount ?? 0
        };
    }
}

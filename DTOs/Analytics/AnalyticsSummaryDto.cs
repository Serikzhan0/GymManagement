namespace GymManagementApi.DTOs;

public class AnalyticsSummaryDto
{
    public int TotalMembers { get; set; }
    public int ActiveMemberships { get; set; }
    public int ExpiredMemberships { get; set; }
    public int TodayCheckIns { get; set; }
    public int TotalWorkoutPlans { get; set; }
    public int TotalExercises { get; set; }
    public string TopTrainerName { get; set; } = "Нет данных";
    public int TopTrainerPlansCount { get; set; }
}

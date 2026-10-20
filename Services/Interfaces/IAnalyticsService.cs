using GymManagementApi.DTOs;

namespace GymManagementApi.Services.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryAsync();
}

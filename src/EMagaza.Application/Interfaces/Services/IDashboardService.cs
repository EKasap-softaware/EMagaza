using EMagaza.Application.Models;

namespace EMagaza.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync();
}

using ConsultantPlatform.Api.Dtos;
using ConsultantPlatform.Api.Dtos.Dashboard;

namespace ConsultantPlatform.Api.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}

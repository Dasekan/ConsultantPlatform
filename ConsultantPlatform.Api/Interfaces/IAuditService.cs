using ConsultantPlatform.Api.Interfaces;

namespace ConsultantPlatform.Api.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string entityType, int entityId, string action, string summary, string userEmail, int? relatedCustomerId = null);
    }
}

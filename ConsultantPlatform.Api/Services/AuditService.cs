using ConsultantPlatform.Api.Data;
using ConsultantPlatform.Api.Models;
using ConsultantPlatform.Api.Interfaces;

namespace ConsultantPlatform.Api.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string entityType, int entityId, string action, string summary, string userEmail, int? relatedCustomerId = null)
        {
            var log = new AuditLog
            {
                EntityType = entityType,
                EntityId = entityId,
                RelatedCustomerId = relatedCustomerId,
                Action = action,
                Summary = summary,
                UserEmail = userEmail,
                CreatedUtc = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}

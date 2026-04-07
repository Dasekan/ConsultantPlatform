namespace ConsultantPlatform.Api.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }

        public int? RelatedCustomerId { get; set; }

        public string Action { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }
    }
}

namespace ConsultantPlatform.Api.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Lead";
        public string ServiceType { get; set; } = "";
        public string CVR { get; set; } = "";
        public string Industry { get; set; } = "";

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public List<Project> Projects { get; set; } = new();

    }
}

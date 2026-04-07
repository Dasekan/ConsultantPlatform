using System.ComponentModel.DataAnnotations;

namespace ConsultantPlatform.Api.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = "";
        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Lead";
        public string ServiceType { get; set; } = "";
        public string CVR { get; set; } = "";
        public string Industry { get; set; } = "";

        public DateTime CreatedUtc { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace ConsultantPlatform.Api.Dtos
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Notes { get; set; }
        public string Status { get; set; } = "Lead";
        public string ServiceType { get; set; } = "";
        public string CVR { get; set; } = "";
        public string Industry { get; set; } = "";
    }
}

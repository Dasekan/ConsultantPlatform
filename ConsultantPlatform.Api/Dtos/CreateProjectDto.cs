using System.ComponentModel.DataAnnotations;

namespace ConsultantPlatform.Api.DTOs.Projects;

public class CreateProjectDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    [Required]
    public string Status { get; set; } = "Lead";

    [Range(0, double.MaxValue, ErrorMessage = "Hourly Rate must not be negative.")]
    public decimal HourlyRate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Project Price must not be negative.")]
    public decimal? ProjectPrice { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public bool InvoiceOverview { get; set; }
}

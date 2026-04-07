using System.ComponentModel.DataAnnotations;

namespace ConsultantPlatform.Api.Models;

public class Project
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    public ProjectStatus Status { get; set; } = ProjectStatus.Lead;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public decimal? ProjectPrice { get; set; }

    public bool InvoiceOverview { get; set; } // true = Yes, false = No

    [Range(0, 100000)]
    public decimal HourlyRate { get; set; }

    // RELATION til Customer:
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}

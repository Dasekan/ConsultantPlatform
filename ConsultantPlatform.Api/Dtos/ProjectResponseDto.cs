namespace ConsultantPlatform.Api.DTOs.Projects;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public decimal HourlyRate { get; set; }

    public decimal? ProjectPrice { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? InvoiceNumber { get; set; }

    public bool InvoiceOverview { get; set; }
    public int CustomerId { get; set; }
}

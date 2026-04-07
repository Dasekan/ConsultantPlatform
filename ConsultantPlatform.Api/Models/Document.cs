using ConsultantPlatform.Api.Models;

namespace ConsultantPlatform.Domain.Entities;

public class CustomerDocument
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public string Name { get; set; } = "";
    public DocumentType Type { get; set; }

    public string? Notes { get; set; }

    // Upload metadata
    public string? OriginalFileName { get; set; }
    public string? StoredFileName { get; set; }   // e.g. "7f4c..._contract.pdf"
    public string? ContentType { get; set; }      // "application/pdf"
    public long? SizeBytes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public enum DocumentType
{
    Contract = 1,
    Onboarding = 2,
    AgreementSummary = 3
}

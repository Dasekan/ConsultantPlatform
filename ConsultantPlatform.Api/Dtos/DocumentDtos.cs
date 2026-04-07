namespace ConsultantPlatform.Api.Dtos
{
    public record DocumentListDto(
    int Id,
    int CustomerId,
    string Name,
    string Type,
    string? Notes,
    string? OriginalFileName,
    string? ContentType,
    long? SizeBytes,
    DateTime CreatedAtUtc
);

}

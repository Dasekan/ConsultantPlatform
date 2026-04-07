using Microsoft.AspNetCore.Http;

namespace ConsultantPlatform.Api.Dtos
{
    public class CreateDocumentDto
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Notes { get; set; }
        public IFormFile? File { get; set; }
    }
}

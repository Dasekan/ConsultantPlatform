using ConsultantPlatform.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantPlatform.Api.Interfaces
{
    public interface IDocumentService
    {
        Task<List<DocumentListDto>> GetForCustomerAsync(int customerId);
        Task<DocumentListDto> CreateAsync(int customerId, CreateDocumentDto form, string userEmail);
        Task<IActionResult> PreviewAsync(int id);
        Task<IActionResult> DownloadAsync(int id);
        Task<bool> DeleteAsync(int id, string userEmail);
    }
}

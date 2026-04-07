using ConsultantPlatform.Api.Dtos;
using ConsultantPlatform.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConsultantPlatform.Api.Controllers;

[ApiController]
[Route("api")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet("customers/{customerId:int}/documents")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<DocumentListDto>>> GetForCustomer(int customerId)
    {
        var docs = await _documentService.GetForCustomerAsync(customerId);
        return Ok(docs);
    }

    [HttpPost("customers/{customerId:int}/documents")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<DocumentListDto>> Create(int customerId, [FromForm] CreateDocumentDto form)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Unknown";

        try
        {
            var created = await _documentService.CreateAsync(customerId, form, userEmail);
            return Ok(created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("documents/{id:int}/preview")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Preview(int id)
    {
        return await _documentService.PreviewAsync(id);
    }

    [HttpGet("documents/{id:int}/download")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Download(int id)
    {
        return await _documentService.DownloadAsync(id);
    }

    [HttpDelete("documents/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "Unknown";

        var deleted = await _documentService.DeleteAsync(id, userEmail);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

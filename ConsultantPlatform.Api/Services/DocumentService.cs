using ConsultantPlatform.Api.Data;
using ConsultantPlatform.Api.Dtos;
using ConsultantPlatform.Api.Interfaces;
using ConsultantPlatform.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ConsultantPlatform.Api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IAuditService _auditService;

        public DocumentService(AppDbContext db, IWebHostEnvironment env, IAuditService auditService)
        {
            _db = db;
            _env = env;
            _auditService = auditService;
        }

        public async Task<List<DocumentListDto>> GetForCustomerAsync(int customerId)
        {
            return await _db.CustomerDocuments
                .Where(d => d.CustomerId == customerId)
                .OrderByDescending(d => d.CreatedAtUtc)
                .Select(d => new DocumentListDto(
                    d.Id,
                    d.CustomerId,
                    d.Name,
                    d.Type.ToString(),
                    d.Notes,
                    d.OriginalFileName,
                    d.ContentType,
                    d.SizeBytes,
                    d.CreatedAtUtc
                ))
                .ToListAsync();
        }

        public async Task<DocumentListDto> CreateAsync(int customerId, CreateDocumentDto form, string userEmail)
        {
            if (string.IsNullOrWhiteSpace(form.Name))
                throw new ArgumentException("Name is required.");

            if (!Enum.TryParse<DocumentType>(form.Type, ignoreCase: true, out var docType))
                throw new ArgumentException("Invalid document type.");

            var customerExists = await _db.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
                throw new KeyNotFoundException("Customer not found.");

            string? storedFileName = null;
            string? originalFileName = null;
            string? contentType = null;
            long? sizeBytes = null;

            if (form.File != null && form.File.Length > 0)
            {
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png"
                };

                var ext = Path.GetExtension(form.File.FileName);
                if (string.IsNullOrWhiteSpace(ext) || !allowed.Contains(ext))
                    throw new ArgumentException("File type not allowed. Allowed: pdf, doc/docx, xls/xlsx, jpg/jpeg, png.");

                storedFileName = $"{Guid.NewGuid():N}{ext}";
                originalFileName = form.File.FileName;
                contentType = string.IsNullOrWhiteSpace(form.File.ContentType)
                    ? "application/octet-stream"
                    : form.File.ContentType;
                sizeBytes = form.File.Length;

                var uploadsRoot = Path.Combine(_env.ContentRootPath, "App_Data", "uploads");
                Directory.CreateDirectory(uploadsRoot);

                var customerFolder = Path.Combine(uploadsRoot, customerId.ToString());
                Directory.CreateDirectory(customerFolder);

                var filePath = Path.Combine(customerFolder, storedFileName);

                await using var stream = System.IO.File.Create(filePath);
                await form.File.CopyToAsync(stream);
            }

            var doc = new CustomerDocument
            {
                CustomerId = customerId,
                Name = form.Name.Trim(),
                Type = docType,
                Notes = string.IsNullOrWhiteSpace(form.Notes) ? null : form.Notes.Trim(),
                OriginalFileName = originalFileName,
                StoredFileName = storedFileName,
                ContentType = contentType,
                SizeBytes = sizeBytes,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.CustomerDocuments.Add(doc);
            await _db.SaveChangesAsync();

            var auditName = string.IsNullOrWhiteSpace(doc.OriginalFileName)
                ? doc.Name
                : doc.OriginalFileName;

            var customer = await _db.Customers.FindAsync(customerId);
            var customerName = customer?.Name ?? $"Customer {customerId}";

            await _auditService.LogAsync(
                "Document",
                doc.Id,
                "Uploaded",
                $"Document '{auditName}' was uploaded for customer '{customerName}'.",
                userEmail,
                doc.CustomerId);

            return new DocumentListDto(
                doc.Id,
                doc.CustomerId,
                doc.Name,
                doc.Type.ToString(),
                doc.Notes,
                doc.OriginalFileName,
                doc.ContentType,
                doc.SizeBytes,
                doc.CreatedAtUtc
            );
        }

        public async Task<IActionResult> PreviewAsync(int id)
        {
            var doc = await _db.CustomerDocuments.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return new NotFoundResult();

            if (doc.CustomerId <= 0 || string.IsNullOrWhiteSpace(doc.StoredFileName))
                return new BadRequestObjectResult("No file uploaded for this document.");

            var path = Path.Combine(
                _env.ContentRootPath,
                "App_Data",
                "uploads",
                doc.CustomerId.ToString(),
                doc.StoredFileName);

            if (!System.IO.File.Exists(path))
                return new NotFoundObjectResult("File missing on server.");

            var ct = doc.ContentType ?? "application/octet-stream";
            return new PhysicalFileResult(path, ct);
        }

        public async Task<IActionResult> DownloadAsync(int id)
        {
            var doc = await _db.CustomerDocuments.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return new NotFoundResult();

            if (doc.CustomerId <= 0 || string.IsNullOrWhiteSpace(doc.StoredFileName))
                return new BadRequestObjectResult("No file uploaded for this document.");

            var path = Path.Combine(
                _env.ContentRootPath,
                "App_Data",
                "uploads",
                doc.CustomerId.ToString(),
                doc.StoredFileName);

            if (!System.IO.File.Exists(path))
                return new NotFoundObjectResult("File missing on server.");

            var ct = doc.ContentType ?? "application/octet-stream";
            var downloadName = string.IsNullOrWhiteSpace(doc.OriginalFileName)
                ? "document"
                : doc.OriginalFileName;

            return new PhysicalFileResult(path, ct)
            {
                FileDownloadName = downloadName
            };
        }

        public async Task<bool> DeleteAsync(int id, string userEmail)
        {
            var doc = await _db.CustomerDocuments.FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null)
                return false;

            if (doc.CustomerId > 0 && !string.IsNullOrWhiteSpace(doc.StoredFileName))
            {
                var path = Path.Combine(
                    _env.ContentRootPath,
                    "App_Data",
                    "uploads",
                    doc.CustomerId.ToString(),
                    doc.StoredFileName);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            var auditName = string.IsNullOrWhiteSpace(doc.OriginalFileName)
                ? doc.Name
                : doc.OriginalFileName;

            _db.CustomerDocuments.Remove(doc);
            await _db.SaveChangesAsync();

            var customer = await _db.Customers.FindAsync(doc.CustomerId);
            var customerName = customer?.Name ?? $"Customer {doc.CustomerId}";

            await _auditService.LogAsync(
                "Document",
                id,
                "Deleted",
                $"Document '{auditName}' was deleted for customer '{customerName}'.",
                userEmail,
                doc.CustomerId);

            return true;
        }
    }
}

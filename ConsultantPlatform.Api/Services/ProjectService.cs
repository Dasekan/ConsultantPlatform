using ConsultantPlatform.Api.Data;
using ConsultantPlatform.Api.DTOs.Projects;
using ConsultantPlatform.Api.Models;
using ConsultantPlatform.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConsultantPlatform.Api.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _auditService;

    public ProjectService(AppDbContext db, IAuditService auditService)
    {
        _db = db;
        _auditService = auditService;
    }

    private static string FormatDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("yyyy-MM-dd") : "empty";
    }

    private static string FormatText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "empty" : value.Trim();
    }

    private static string FormatYesNo(bool value)
    {
        return value ? "Yes" : "No";
    }

    private static ProjectStatus ParseStatus(string status)
    {
        // accepter "Lead", "Active", "Completed", "Invoiced" (case-insensitive)
        if (Enum.TryParse<ProjectStatus>(status, ignoreCase: true, out var parsed))
            return parsed;

        throw new ArgumentException("Invalid status. Use Lead, Active, Completed, Invoiced.");
    }

    public async Task<List<ProjectResponseDto>> GetByCustomerAsync(int customerId, string? status = null)
    {
        var query = _db.Projects
            .Where(p => p.CustomerId == customerId)
            .AsQueryable();

        // FILTER
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<ProjectStatus>(status, true, out var parsed))
            {
                query = query.Where(p => p.Status == parsed);
            }
        }

        var items = await query
            .OrderByDescending(p => p.Id)
            .ToListAsync();

        return items.Select(p => new ProjectResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Status = p.Status.ToString(),
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            HourlyRate = p.HourlyRate,
            ProjectPrice = p.ProjectPrice,
            InvoiceNumber = p.InvoiceNumber,
            InvoiceOverview = p.InvoiceOverview,
            CustomerId = p.CustomerId
        }).ToList();
    }

    public async Task<ProjectResponseDto> CreateAsync(int customerId, CreateProjectDto dto, string userEmail)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        if (customer == null) throw new KeyNotFoundException("Customer not found.");

        if (dto.EndDate < dto.StartDate)
            throw new ArgumentException("End date must be the same as or after start date.");

        if (!string.IsNullOrWhiteSpace(dto.InvoiceNumber))
        {
            var invoiceNumberExists = await _db.Projects
                .AnyAsync(p => p.InvoiceNumber == dto.InvoiceNumber);

            if (invoiceNumberExists)
                throw new ArgumentException("Invoice number already exists. It must be unique.");
        }

        var project = new Project
        {
            Name = dto.Name,
            Status = ParseStatus(dto.Status),

            StartDate = dto.StartDate,
            EndDate = dto.EndDate,

            HourlyRate = dto.HourlyRate,
            ProjectPrice = dto.ProjectPrice,

            InvoiceNumber = dto.InvoiceNumber,
            InvoiceOverview = dto.InvoiceOverview,

            CustomerId = customerId
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        await _auditService.LogAsync(
            "Project",
            project.Id,
            "Created",
            $"Project '{project.Name}' was created for customer '{customer.Name}'.",
            userEmail,
            project.CustomerId);

        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Status = project.Status.ToString(),

            StartDate = project.StartDate,
            EndDate = project.EndDate,

            HourlyRate = project.HourlyRate,
            ProjectPrice = project.ProjectPrice,

            InvoiceNumber = project.InvoiceNumber,
            InvoiceOverview = project.InvoiceOverview,

            CustomerId = project.CustomerId
        };
    }

    public async Task<ProjectResponseDto?> GetAsync(int id)
    {
        var p = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return null;

        return new ProjectResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Status = p.Status.ToString(),

            StartDate = p.StartDate,
            EndDate = p.EndDate,

            HourlyRate = p.HourlyRate,
            ProjectPrice = p.ProjectPrice,

            InvoiceNumber = p.InvoiceNumber,
            InvoiceOverview = p.InvoiceOverview,

            CustomerId = p.CustomerId
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateProjectDto dto, string userEmail)
    {
        var p = await _db.Projects.FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return false;

        var oldName = p.Name;
        var oldStatus = p.Status;
        var oldStartDate = p.StartDate;
        var oldEndDate = p.EndDate;
        var oldHourlyRate = p.HourlyRate;
        var oldProjectPrice = p.ProjectPrice;
        var oldInvoiceNumber = p.InvoiceNumber;
        var oldInvoiceOverview = p.InvoiceOverview;

        var newStatus = ParseStatus(dto.Status);

        if (newStatus == ProjectStatus.Completed)
        {
            dto.StartDate = null;
            dto.EndDate = null;
        }
        else
        {
            if (!dto.StartDate.HasValue || !dto.EndDate.HasValue)
                throw new ArgumentException("Start date and end date are required unless status is Completed.");

            if (dto.EndDate.Value < dto.StartDate.Value)
                throw new ArgumentException("End date must be the same as or after start date.");
        }

        if (!string.IsNullOrWhiteSpace(dto.InvoiceNumber))
        {
            var invoiceNumberExists = await _db.Projects
                .AnyAsync(project => project.InvoiceNumber == dto.InvoiceNumber && project.Id != id);

            if (invoiceNumberExists)
                throw new ArgumentException("Invoice number already exists. It must be unique.");
        }

        p.Name = dto.Name;
        p.Status = newStatus;
        p.StartDate = dto.StartDate;
        p.EndDate = dto.EndDate;
        p.HourlyRate = dto.HourlyRate;
        p.ProjectPrice = dto.ProjectPrice;
        p.InvoiceNumber = dto.InvoiceNumber;
        p.InvoiceOverview = dto.InvoiceOverview;

        var changes = new List<string>();

        if (oldName != p.Name)
            changes.Add($"name changed from '{oldName}' to '{p.Name}'");

        if (oldStatus != p.Status)
            changes.Add($"status changed from '{oldStatus}' to '{p.Status}'");

        if (oldStartDate != p.StartDate)
            changes.Add($"start date changed from '{FormatDate(oldStartDate)}' to '{FormatDate(p.StartDate)}'");

        if (oldEndDate != p.EndDate)
            changes.Add($"end date changed from '{FormatDate(oldEndDate)}' to '{FormatDate(p.EndDate)}'");

        if (oldHourlyRate != p.HourlyRate)
            changes.Add($"hourly rate changed from '{oldHourlyRate}' to '{p.HourlyRate}'");

        if (oldProjectPrice != p.ProjectPrice)
            changes.Add($"project price changed from '{oldProjectPrice}' to '{p.ProjectPrice}'");

        if (FormatText(oldInvoiceNumber) != FormatText(p.InvoiceNumber))
            changes.Add($"invoice number changed from '{FormatText(oldInvoiceNumber)}' to '{FormatText(p.InvoiceNumber)}'");

        if (oldInvoiceOverview != p.InvoiceOverview)
            changes.Add($"invoice overview changed from '{FormatYesNo(oldInvoiceOverview)}' to '{FormatYesNo(p.InvoiceOverview)}'");

        await _db.SaveChangesAsync();

        var summary = changes.Count > 0
            ? $"Project '{p.Name}' was updated: {string.Join(", ", changes)}."
            : $"Project '{p.Name}' was updated.";

        await _auditService.LogAsync(
            "Project",
            p.Id,
            "Updated",
            summary,
            userEmail,
            p.CustomerId);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, string userEmail)
    {
        var p = await _db.Projects
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (p == null) return false;

        var hasInvoiceData =
            !string.IsNullOrWhiteSpace(p.InvoiceNumber) ||
            p.InvoiceOverview;

        if (hasInvoiceData)
            throw new ArgumentException("This project cannot be deleted because it contains invoice data.");

        var projectName = p.Name;

        _db.Projects.Remove(p);
        await _db.SaveChangesAsync();

        var customerName = p.Customer?.Name ?? $"Customer {p.CustomerId}";

        await _auditService.LogAsync(
            "Project",
            id,
            "Deleted",
            $"Project '{projectName}' was deleted for customer '{customerName}'.",
            userEmail,
            p.CustomerId);

        return true;
    }
}

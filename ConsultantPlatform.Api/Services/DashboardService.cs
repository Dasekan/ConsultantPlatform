using ConsultantPlatform.Api.Data;
using ConsultantPlatform.Api.Dtos.Dashboard;
using ConsultantPlatform.Api.Interfaces;
using ConsultantPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantPlatform.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var summary = new DashboardSummaryDto();

        // ---------- CUSTOMERS ----------
        summary.Customers.Total = await _db.Customers.CountAsync();
        summary.Customers.Lead = await _db.Customers.CountAsync(c => c.Status == "Lead");
        summary.Customers.Active = await _db.Customers.CountAsync(c => c.Status == "Active");
        summary.Customers.ProjectClient = await _db.Customers.CountAsync(c =>
            c.Status == "Project Client" || c.Status == "Project client");
        summary.Customers.Inactive = await _db.Customers.CountAsync(c => c.Status == "Inactive");

        // ---------- PROJECTS ----------
        summary.Projects.Total = await _db.Projects.CountAsync();
        summary.Projects.Lead = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.Lead);
        summary.Projects.Active = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.Active);
        summary.Projects.Completed = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.Completed);
        summary.Projects.Invoiced = await _db.Projects.CountAsync(p => p.Status == ProjectStatus.Invoiced);

        // ---------- UPCOMING DEADLINES ----------
        summary.UpcomingDeadlines = await _db.Projects
            .Include(p => p.Customer)
            .Where(p => p.EndDate != null && p.Status != ProjectStatus.Completed)
            .OrderBy(p => p.EndDate)
            .Take(5)
            .Select(p => new UpcomingProjectDto
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                CustomerName = p.Customer != null ? p.Customer.Name : "",
                EndDate = p.EndDate,
                Status = p.Status.ToString()
            })
            .ToListAsync();

        // ---------- RECENT ACTIVITY ----------
        var recentAuditLogs = await _db.AuditLogs
    .OrderByDescending(a => a.CreatedUtc)
    .Take(10)
    .ToListAsync();

        summary.RecentActivity = recentAuditLogs
            .Select(a => new RecentActivityDto
            {
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                RelatedCustomerId = a.RelatedCustomerId,
                Action = a.Action,
                Summary = a.Summary,
                UserEmail = a.UserEmail,
                CreatedUtc = a.CreatedUtc
            })
            .ToList();

        return summary;
    }
}

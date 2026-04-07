using ConsultantPlatform.Api.Data;
using ConsultantPlatform.Api.Dtos;
using ConsultantPlatform.Api.Models;
using ConsultantPlatform.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConsultantPlatform.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _db;
        private readonly IAuditService _auditService;

        public CustomerService(AppDbContext db, IAuditService auditService)
        {
            _db = db;
            _auditService = auditService;
        }

        private static string FormatText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "empty" : value.Trim();
        }

        public async Task<List<CustomerDto>> GetAllAsync(string? search = null, string? sort = null, string? status = null)
        {
            var query = _db.Customers.AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();

                query = query.Where(c =>
                    c.Name.ToLower().Contains(s) ||
                    (c.Email != null && c.Email.ToLower().Contains(s)) ||
                    (c.Industry != null && c.Industry.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToLower();

                query = query.Where(c =>
                    c.Status != null &&
                    c.Status.ToLower() == normalizedStatus);
            }

            // SORTING
            query = sort switch
            {
                "name" => query.OrderBy(c => c.Name),
                "email" => query.OrderBy(c => c.Email),
                "created" => query.OrderByDescending(c => c.CreatedUtc),
                _ => query.OrderByDescending(c => c.CreatedUtc)
            };

            return await query
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    Notes = c.Notes,
                    Status = c.Status,
                    ServiceType = c.ServiceType,
                    CVR = c.CVR,
                    Industry = c.Industry,
                    CreatedUtc = c.CreatedUtc
                })
                .ToListAsync();
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, string userEmail)
        {

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            var normalizedEmail = dto.Email.Trim().ToLower();

            var emailExists = await _db.Customers
                .AnyAsync(c => c.Email != null && c.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                throw new ArgumentException("Email already exists. It must be unique.");
            }
            var customer = new Customer
            {
                Name = dto.Name.Trim(),
                Email = dto.Email,
                Phone = dto.Phone,
                Notes = dto.Notes,

                // NYE FELTER
                Status = dto.Status,
                ServiceType = dto.ServiceType,
                CVR = dto.CVR,
                Industry = dto.Industry
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            await _auditService.LogAsync(
                "Customer",
                customer.Id,
                "Created",
                $"Customer '{customer.Name}' was created.",
                userEmail,
                customer.Id);

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Notes = customer.Notes,

                // NYE FELTER
                Status = customer.Status,
                ServiceType = customer.ServiceType,
                CVR = customer.CVR,
                Industry = customer.Industry,

                CreatedUtc = customer.CreatedUtc
            };
        }

        public async Task<bool> DeleteAsync(int id, string userEmail)
        {
            var customer = await _db.Customers.FindAsync(id);
            if (customer == null) return false;

            var customerName = customer.Name;

            _db.Customers.Remove(customer);
            await _db.SaveChangesAsync();

            await _auditService.LogAsync(
                "Customer",
                id,
                "Deleted",
                $"Customer '{customerName}' was deleted.",
                userEmail,
                id);

            return true;
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return null;

            return new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Notes = c.Notes,

                // NYE FELTER
                Status = c.Status,
                ServiceType = c.ServiceType,
                CVR = c.CVR,
                Industry = c.Industry,

                CreatedUtc = c.CreatedUtc
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto, string userEmail)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            var normalizedEmail = dto.Email.Trim().ToLower();

            var emailExists = await _db.Customers
                .AnyAsync(customer => customer.Email != null &&
                                      customer.Email.ToLower() == normalizedEmail &&
                                      customer.Id != id);

            if (emailExists)
            {
                throw new ArgumentException("Email already exists. It must be unique.");
            }

            var oldName = c.Name;
            var oldEmail = c.Email;
            var oldPhone = c.Phone;
            var oldStatus = c.Status;
            var oldServiceType = c.ServiceType;
            var oldCvr = c.CVR;
            var oldIndustry = c.Industry;
            var oldNotes = c.Notes;

            c.Name = dto.Name.Trim();
            c.Email = dto.Email;
            c.Phone = dto.Phone;
            c.Notes = dto.Notes;
            c.Status = dto.Status;
            c.ServiceType = dto.ServiceType;
            c.CVR = dto.CVR;
            c.Industry = dto.Industry;

            var changes = new List<string>();

            if (FormatText(oldName) != FormatText(c.Name))
                changes.Add($"name changed from '{FormatText(oldName)}' to '{FormatText(c.Name)}'");

            if (FormatText(oldEmail) != FormatText(c.Email))
                changes.Add($"email changed from '{FormatText(oldEmail)}' to '{FormatText(c.Email)}'");

            if (FormatText(oldPhone) != FormatText(c.Phone))
                changes.Add($"phone changed from '{FormatText(oldPhone)}' to '{FormatText(c.Phone)}'");

            if (FormatText(oldStatus) != FormatText(c.Status))
                changes.Add($"status changed from '{FormatText(oldStatus)}' to '{FormatText(c.Status)}'");

            if (FormatText(oldServiceType) != FormatText(c.ServiceType))
                changes.Add($"service type changed from '{FormatText(oldServiceType)}' to '{FormatText(c.ServiceType)}'");

            if (FormatText(oldCvr) != FormatText(c.CVR))
                changes.Add($"CVR changed from '{FormatText(oldCvr)}' to '{FormatText(c.CVR)}'");

            if (FormatText(oldIndustry) != FormatText(c.Industry))
                changes.Add($"industry changed from '{FormatText(oldIndustry)}' to '{FormatText(c.Industry)}'");

            if (FormatText(oldNotes) != FormatText(c.Notes))
                changes.Add("notes were updated");

            await _db.SaveChangesAsync();

            var summary = changes.Count > 0
                ? $"Customer '{c.Name}' was updated: {string.Join(", ", changes)}."
                : $"Customer '{c.Name}' was updated.";

            await _auditService.LogAsync(
                "Customer",
                c.Id,
                "Updated",
                summary,
                userEmail,
                c.Id);

            return true;
        }
    }
}

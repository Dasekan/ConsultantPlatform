using ConsultantPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ConsultantPlatform.Domain.Entities;

namespace ConsultantPlatform.Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = default!;

        public DbSet<Project> Projects => Set<Project>();

        public DbSet<CustomerDocument> CustomerDocuments => Set<CustomerDocument>();

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasIndex(p => p.InvoiceNumber)
                .IsUnique();

            builder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }

}

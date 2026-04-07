using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantPlatform.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatedCustomerIdToAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedCustomerId",
                table: "AuditLogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedCustomerId",
                table: "AuditLogs");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Notifications.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedOrderEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedOrderEvents",
                schema: "notifications",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedOrderEvents", x => x.EventId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedOrderEvents",
                schema: "notifications");
        }
    }
}

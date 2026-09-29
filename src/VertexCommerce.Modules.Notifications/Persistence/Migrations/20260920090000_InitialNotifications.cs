using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Notifications.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialNotifications : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "notifications");

        migrationBuilder.CreateTable(
            name: "Notifications",
            schema: "notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                PayloadJson = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PushSubscriptions",
            schema: "notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Endpoint = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                P256dhKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                AuthKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PushSubscriptions", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_UserId_CreatedAt",
            schema: "notifications",
            table: "Notifications",
            columns: new[] { "UserId", "CreatedAt" });

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_UserId_IsRead",
            schema: "notifications",
            table: "Notifications",
            columns: new[] { "UserId", "IsRead" });

        migrationBuilder.CreateIndex(
            name: "IX_PushSubscriptions_Endpoint",
            schema: "notifications",
            table: "PushSubscriptions",
            column: "Endpoint",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PushSubscriptions_UserId",
            schema: "notifications",
            table: "PushSubscriptions",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Notifications",
            schema: "notifications");

        migrationBuilder.DropTable(
            name: "PushSubscriptions",
            schema: "notifications");
    }
}

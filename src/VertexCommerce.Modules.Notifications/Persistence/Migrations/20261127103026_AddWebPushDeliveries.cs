using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Notifications.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWebPushDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PayloadJson",
                schema: "notifications",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PushDeliveries",
                schema: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    NextAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    LeaseId = table.Column<Guid>(type: "uuid", nullable: true),
                    LeaseExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Outcome = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LastHttpStatus = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushDeliveries_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalSchema: "notifications",
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PushDeliveries_NextAttemptAt_Id",
                schema: "notifications",
                table: "PushDeliveries",
                columns: new[] { "NextAttemptAt", "Id" },
                filter: "\"CompletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PushDeliveries_NotificationId_SubscriptionId",
                schema: "notifications",
                table: "PushDeliveries",
                columns: new[] { "NotificationId", "SubscriptionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PushDeliveries",
                schema: "notifications");

            migrationBuilder.AlterColumn<string>(
                name: "PayloadJson",
                schema: "notifications",
                table: "Notifications",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}

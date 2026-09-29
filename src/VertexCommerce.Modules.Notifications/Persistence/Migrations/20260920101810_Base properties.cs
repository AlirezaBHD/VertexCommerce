using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Notifications.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Baseproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "notifications",
                table: "PushSubscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "notifications",
                table: "PushSubscriptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "notifications",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "notifications",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "notifications",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "notifications",
                table: "PushSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "notifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "notifications",
                table: "Notifications");
        }
    }
}

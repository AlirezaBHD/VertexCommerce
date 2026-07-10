using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixeoutboxMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedOnUtc",
                schema: "identity",
                table: "OutboxMessages",
                newName: "ProcessedOn");

            migrationBuilder.RenameColumn(
                name: "OccurredOnUtc",
                schema: "identity",
                table: "OutboxMessages",
                newName: "OccurredOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedOn",
                schema: "identity",
                table: "OutboxMessages",
                newName: "ProcessedOnUtc");

            migrationBuilder.RenameColumn(
                name: "OccurredOn",
                schema: "identity",
                table: "OutboxMessages",
                newName: "OccurredOnUtc");
        }
    }
}

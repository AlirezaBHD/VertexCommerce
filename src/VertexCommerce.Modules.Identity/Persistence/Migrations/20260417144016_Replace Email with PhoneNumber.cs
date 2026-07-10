using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Identity.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceEmailwithPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                schema: "identity",
                table: "Users",
                newName: "PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                newName: "IX_Users_PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                schema: "identity",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PhoneNumber",
                schema: "identity",
                table: "Users",
                newName: "IX_Users_Email");
        }
    }
}

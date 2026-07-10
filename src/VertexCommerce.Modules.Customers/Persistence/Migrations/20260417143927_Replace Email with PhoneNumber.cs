using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Customers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceEmailwithPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                schema: "customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "customers",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "customers",
                table: "Customers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                schema: "customers",
                table: "Customers",
                column: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_PhoneNumber",
                schema: "customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "customers",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "customers",
                table: "Customers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "customers",
                table: "Customers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                schema: "customers",
                table: "Customers",
                column: "Email");
        }
    }
}

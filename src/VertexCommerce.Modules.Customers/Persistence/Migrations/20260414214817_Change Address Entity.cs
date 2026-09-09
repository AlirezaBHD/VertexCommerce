using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Customers.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAddressEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "Street",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.RenameColumn(
                name: "State",
                schema: "customers",
                table: "CustomerAddresses",
                newName: "Province");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "customers",
                table: "CustomerAddresses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "customers",
                table: "CustomerAddresses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PostalAddress",
                schema: "customers",
                table: "CustomerAddresses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                schema: "customers",
                table: "CustomerAddresses",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_PostalCode",
                schema: "customers",
                table: "CustomerAddresses",
                column: "PostalCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerAddresses_PostalCode",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "PostalAddress",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                schema: "customers",
                table: "CustomerAddresses");

            migrationBuilder.RenameColumn(
                name: "Province",
                schema: "customers",
                table: "CustomerAddresses",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "customers",
                table: "CustomerAddresses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                schema: "customers",
                table: "CustomerAddresses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                schema: "customers",
                table: "CustomerAddresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Orders.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingCountry",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingState",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingStreet",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingStreet",
                schema: "orders",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippingZipCode",
                schema: "orders",
                table: "Orders",
                newName: "ShippingPostalCode");

            migrationBuilder.RenameColumn(
                name: "ShippingState",
                schema: "orders",
                table: "Orders",
                newName: "ShippingProvince");

            migrationBuilder.RenameColumn(
                name: "ShippingCountry",
                schema: "orders",
                table: "Orders",
                newName: "BillingProvince");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                schema: "orders",
                table: "Orders",
                newName: "CustomerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "BillingZipCode",
                schema: "orders",
                table: "Orders",
                newName: "BillingPostalCode");

            migrationBuilder.AddColumn<string>(
                name: "BillingLabel",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BillingLatitude",
                schema: "orders",
                table: "Orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BillingLongitude",
                schema: "orders",
                table: "Orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "BillingPostalAddress",
                schema: "orders",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingLabel",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ShippingLatitude",
                schema: "orders",
                table: "Orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ShippingLongitude",
                schema: "orders",
                table: "Orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPostalAddress",
                schema: "orders",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "VariantId",
                schema: "orders",
                table: "order_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingLabel",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingLatitude",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingLongitude",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BillingPostalAddress",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingLabel",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingLatitude",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingLongitude",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingPostalAddress",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VariantId",
                schema: "orders",
                table: "order_items");

            migrationBuilder.RenameColumn(
                name: "ShippingProvince",
                schema: "orders",
                table: "Orders",
                newName: "ShippingState");

            migrationBuilder.RenameColumn(
                name: "ShippingPostalCode",
                schema: "orders",
                table: "Orders",
                newName: "ShippingZipCode");

            migrationBuilder.RenameColumn(
                name: "CustomerPhoneNumber",
                schema: "orders",
                table: "Orders",
                newName: "CustomerEmail");

            migrationBuilder.RenameColumn(
                name: "BillingProvince",
                schema: "orders",
                table: "Orders",
                newName: "ShippingCountry");

            migrationBuilder.RenameColumn(
                name: "BillingPostalCode",
                schema: "orders",
                table: "Orders",
                newName: "BillingZipCode");

            migrationBuilder.AddColumn<string>(
                name: "BillingCountry",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingState",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingStreet",
                schema: "orders",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingStreet",
                schema: "orders",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}

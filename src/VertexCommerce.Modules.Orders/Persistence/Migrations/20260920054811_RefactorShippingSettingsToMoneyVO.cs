using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Orders.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorShippingSettingsToMoneyVO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FreeShippingThreshold",
                schema: "orders",
                table: "ShippingSettings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostCurrency",
                schema: "orders",
                table: "ShippingSettings",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "FreeShippingThresholdCurrency",
                schema: "orders",
                table: "ShippingSettings",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostCurrency",
                schema: "orders",
                table: "ShippingSettings");

            migrationBuilder.DropColumn(
                name: "FreeShippingThresholdCurrency",
                schema: "orders",
                table: "ShippingSettings");

            migrationBuilder.AlterColumn<decimal>(
                name: "FreeShippingThreshold",
                schema: "orders",
                table: "ShippingSettings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}

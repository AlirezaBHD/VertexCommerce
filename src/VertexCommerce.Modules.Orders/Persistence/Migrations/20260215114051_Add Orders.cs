using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Orders.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_order_id",
                schema: "orders",
                table: "order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orders",
                schema: "orders",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_created_at",
                schema: "orders",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_status",
                schema: "orders",
                table: "orders");

            migrationBuilder.RenameTable(
                name: "orders",
                schema: "orders",
                newName: "Orders",
                newSchema: "orders");

            migrationBuilder.RenameColumn(
                name: "status",
                schema: "orders",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                schema: "orders",
                table: "Orders",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "orders",
                table: "Orders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                schema: "orders",
                table: "Orders",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "shipped_at",
                schema: "orders",
                table: "Orders",
                newName: "ShippedAt");

            migrationBuilder.RenameColumn(
                name: "payment_status",
                schema: "orders",
                table: "Orders",
                newName: "PaymentStatus");

            migrationBuilder.RenameColumn(
                name: "order_number",
                schema: "orders",
                table: "Orders",
                newName: "OrderNumber");

            migrationBuilder.RenameColumn(
                name: "delivered_at",
                schema: "orders",
                table: "Orders",
                newName: "DeliveredAt");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                schema: "orders",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "customer_email",
                schema: "orders",
                table: "Orders",
                newName: "CustomerEmail");

            migrationBuilder.RenameColumn(
                name: "created_at",
                schema: "orders",
                table: "Orders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "cancelled_at",
                schema: "orders",
                table: "Orders",
                newName: "CancelledAt");

            migrationBuilder.RenameColumn(
                name: "cancellation_reason",
                schema: "orders",
                table: "Orders",
                newName: "CancellationReason");

            migrationBuilder.RenameColumn(
                name: "total_currency",
                schema: "orders",
                table: "Orders",
                newName: "TotalAmountCurrency");

            migrationBuilder.RenameColumn(
                name: "total_amount",
                schema: "orders",
                table: "Orders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "tax_currency",
                schema: "orders",
                table: "Orders",
                newName: "TaxCurrency");

            migrationBuilder.RenameColumn(
                name: "tax_amount",
                schema: "orders",
                table: "Orders",
                newName: "Tax");

            migrationBuilder.RenameColumn(
                name: "sub_total_currency",
                schema: "orders",
                table: "Orders",
                newName: "SubTotalCurrency");

            migrationBuilder.RenameColumn(
                name: "sub_total_amount",
                schema: "orders",
                table: "Orders",
                newName: "SubTotal");

            migrationBuilder.RenameColumn(
                name: "shipping_zip_code",
                schema: "orders",
                table: "Orders",
                newName: "ShippingZipCode");

            migrationBuilder.RenameColumn(
                name: "shipping_street",
                schema: "orders",
                table: "Orders",
                newName: "ShippingStreet");

            migrationBuilder.RenameColumn(
                name: "shipping_state",
                schema: "orders",
                table: "Orders",
                newName: "ShippingState");

            migrationBuilder.RenameColumn(
                name: "shipping_country",
                schema: "orders",
                table: "Orders",
                newName: "ShippingCountry");

            migrationBuilder.RenameColumn(
                name: "shipping_cost_currency",
                schema: "orders",
                table: "Orders",
                newName: "ShippingCostCurrency");

            migrationBuilder.RenameColumn(
                name: "shipping_cost_amount",
                schema: "orders",
                table: "Orders",
                newName: "ShippingCost");

            migrationBuilder.RenameColumn(
                name: "shipping_city",
                schema: "orders",
                table: "Orders",
                newName: "ShippingCity");

            migrationBuilder.RenameColumn(
                name: "billing_zip_code",
                schema: "orders",
                table: "Orders",
                newName: "BillingZipCode");

            migrationBuilder.RenameColumn(
                name: "billing_street",
                schema: "orders",
                table: "Orders",
                newName: "BillingStreet");

            migrationBuilder.RenameColumn(
                name: "billing_state",
                schema: "orders",
                table: "Orders",
                newName: "BillingState");

            migrationBuilder.RenameColumn(
                name: "billing_country",
                schema: "orders",
                table: "Orders",
                newName: "BillingCountry");

            migrationBuilder.RenameColumn(
                name: "billing_city",
                schema: "orders",
                table: "Orders",
                newName: "BillingCity");

            migrationBuilder.RenameIndex(
                name: "IX_orders_order_number",
                schema: "orders",
                table: "Orders",
                newName: "IX_Orders_OrderNumber");

            migrationBuilder.RenameIndex(
                name: "IX_orders_customer_id",
                schema: "orders",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "orders",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                schema: "orders",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                schema: "orders",
                table: "Orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillingZipCode",
                schema: "orders",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillingStreet",
                schema: "orders",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillingState",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillingCountry",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BillingCity",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                schema: "orders",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessingAt",
                schema: "orders",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                schema: "orders",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                schema: "orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_Orders_order_id",
                schema: "orders",
                table: "order_items",
                column: "order_id",
                principalSchema: "orders",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_Orders_order_id",
                schema: "orders",
                table: "order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProcessingAt",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                schema: "orders",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "orders",
                newName: "orders",
                newSchema: "orders");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "orders",
                table: "orders",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                schema: "orders",
                table: "orders",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "orders",
                table: "orders",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "orders",
                table: "orders",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ShippedAt",
                schema: "orders",
                table: "orders",
                newName: "shipped_at");

            migrationBuilder.RenameColumn(
                name: "PaymentStatus",
                schema: "orders",
                table: "orders",
                newName: "payment_status");

            migrationBuilder.RenameColumn(
                name: "OrderNumber",
                schema: "orders",
                table: "orders",
                newName: "order_number");

            migrationBuilder.RenameColumn(
                name: "DeliveredAt",
                schema: "orders",
                table: "orders",
                newName: "delivered_at");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "orders",
                table: "orders",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CustomerEmail",
                schema: "orders",
                table: "orders",
                newName: "customer_email");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "orders",
                table: "orders",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                schema: "orders",
                table: "orders",
                newName: "cancelled_at");

            migrationBuilder.RenameColumn(
                name: "CancellationReason",
                schema: "orders",
                table: "orders",
                newName: "cancellation_reason");

            migrationBuilder.RenameColumn(
                name: "TotalAmountCurrency",
                schema: "orders",
                table: "orders",
                newName: "total_currency");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                schema: "orders",
                table: "orders",
                newName: "total_amount");

            migrationBuilder.RenameColumn(
                name: "TaxCurrency",
                schema: "orders",
                table: "orders",
                newName: "tax_currency");

            migrationBuilder.RenameColumn(
                name: "Tax",
                schema: "orders",
                table: "orders",
                newName: "tax_amount");

            migrationBuilder.RenameColumn(
                name: "SubTotalCurrency",
                schema: "orders",
                table: "orders",
                newName: "sub_total_currency");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                schema: "orders",
                table: "orders",
                newName: "sub_total_amount");

            migrationBuilder.RenameColumn(
                name: "ShippingZipCode",
                schema: "orders",
                table: "orders",
                newName: "shipping_zip_code");

            migrationBuilder.RenameColumn(
                name: "ShippingStreet",
                schema: "orders",
                table: "orders",
                newName: "shipping_street");

            migrationBuilder.RenameColumn(
                name: "ShippingState",
                schema: "orders",
                table: "orders",
                newName: "shipping_state");

            migrationBuilder.RenameColumn(
                name: "ShippingCountry",
                schema: "orders",
                table: "orders",
                newName: "shipping_country");

            migrationBuilder.RenameColumn(
                name: "ShippingCostCurrency",
                schema: "orders",
                table: "orders",
                newName: "shipping_cost_currency");

            migrationBuilder.RenameColumn(
                name: "ShippingCost",
                schema: "orders",
                table: "orders",
                newName: "shipping_cost_amount");

            migrationBuilder.RenameColumn(
                name: "ShippingCity",
                schema: "orders",
                table: "orders",
                newName: "shipping_city");

            migrationBuilder.RenameColumn(
                name: "BillingZipCode",
                schema: "orders",
                table: "orders",
                newName: "billing_zip_code");

            migrationBuilder.RenameColumn(
                name: "BillingStreet",
                schema: "orders",
                table: "orders",
                newName: "billing_street");

            migrationBuilder.RenameColumn(
                name: "BillingState",
                schema: "orders",
                table: "orders",
                newName: "billing_state");

            migrationBuilder.RenameColumn(
                name: "BillingCountry",
                schema: "orders",
                table: "orders",
                newName: "billing_country");

            migrationBuilder.RenameColumn(
                name: "BillingCity",
                schema: "orders",
                table: "orders",
                newName: "billing_city");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OrderNumber",
                schema: "orders",
                table: "orders",
                newName: "IX_orders_order_number");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                schema: "orders",
                table: "orders",
                newName: "IX_orders_customer_id");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                schema: "orders",
                table: "orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "payment_status",
                schema: "orders",
                table: "orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "customer_email",
                schema: "orders",
                table: "orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "billing_zip_code",
                schema: "orders",
                table: "orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "billing_street",
                schema: "orders",
                table: "orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "billing_state",
                schema: "orders",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "billing_country",
                schema: "orders",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "billing_city",
                schema: "orders",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_orders",
                schema: "orders",
                table: "orders",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_created_at",
                schema: "orders",
                table: "orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_orders_status",
                schema: "orders",
                table: "orders",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_order_id",
                schema: "orders",
                table: "order_items",
                column: "order_id",
                principalSchema: "orders",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

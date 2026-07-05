using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RefactoreCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_name",
                schema: "catalog",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_categories_name",
                schema: "catalog",
                table: "categories");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "catalog",
                table: "categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_image_path",
                schema: "catalog",
                table: "categories",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "icon_path",
                schema: "catalog",
                table: "categories",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_alt_text",
                schema: "catalog",
                table: "categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "include_in_menu",
                schema: "catalog",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "seo_keywords",
                schema: "catalog",
                table: "categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "seo_meta_description",
                schema: "catalog",
                table: "categories",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "seo_meta_title",
                schema: "catalog",
                table: "categories",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "seo_slug",
                schema: "catalog",
                table: "categories",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "show_on_home",
                schema: "catalog",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cover_image_path",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "icon_path",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "image_alt_text",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "include_in_menu",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "seo_keywords",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "seo_meta_description",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "seo_meta_title",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "seo_slug",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "show_on_home",
                schema: "catalog",
                table: "categories");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "catalog",
                table: "categories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_products_name",
                schema: "catalog",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_categories_name",
                schema: "catalog",
                table: "categories",
                column: "name");
        }
    }
}

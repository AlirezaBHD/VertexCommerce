using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryActiveUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_categories_name_active",
                schema: "catalog",
                table: "categories",
                column: "name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_categories_seo_slug_active",
                schema: "catalog",
                table: "categories",
                column: "seo_slug",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_categories_name_active",
                schema: "catalog",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_seo_slug_active",
                schema: "catalog",
                table: "categories");
        }
    }
}

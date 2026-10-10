using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddColorHexToCatalogAttributeOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "color_hex",
                schema: "catalog",
                table: "catalog_attribute_options",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.Sql("""
                INSERT INTO catalog.catalog_attributes (id, code, default_name, type, created_at, updated_at, "IsDeleted")
                VALUES (gen_random_uuid(), 'color', 'رنگ', NULL, CURRENT_TIMESTAMP, NULL, FALSE)
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO catalog.catalog_attribute_options (id, attribute_id, code, default_name, media_path, color_hex, created_at, updated_at, "IsDeleted")
                SELECT gen_random_uuid(), catalog_attribute.id, seed.code, seed.default_name, NULL, NULL, CURRENT_TIMESTAMP, NULL, FALSE
                FROM catalog.catalog_attributes AS catalog_attribute
                CROSS JOIN (VALUES
                    ('black', 'مشکی', '#000000'),
                    ('white', 'سفید', '#FFFFFF'),
                    ('red', 'قرمز', '#FF0000'),
                    ('blue', 'آبی', '#0000FF'),
                    ('green', 'سبز', '#008000'),
                    ('yellow', 'زرد', '#FFFF00'),
                    ('gray', 'خاکستری', '#808080'),
                    ('brown', 'قهوه‌ای', '#8B4513')
                ) AS seed(code, default_name, color_hex)
                WHERE catalog_attribute.code = 'color'
                ON CONFLICT (attribute_id, code) DO NOTHING;

                UPDATE catalog.catalog_attribute_options AS catalog_option
                SET color_hex = seed.color_hex, updated_at = CURRENT_TIMESTAMP
                FROM catalog.catalog_attributes AS catalog_attribute
                CROSS JOIN (VALUES
                    ('black', '#000000'),
                    ('white', '#FFFFFF'),
                    ('red', '#FF0000'),
                    ('blue', '#0000FF'),
                    ('green', '#008000'),
                    ('yellow', '#FFFF00'),
                    ('gray', '#808080'),
                    ('brown', '#8B4513')
                ) AS seed(code, color_hex)
                WHERE catalog_attribute.code = 'color'
                    AND catalog_option.attribute_id = catalog_attribute.id
                    AND catalog_option.code = seed.code
                    AND catalog_option.color_hex IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color_hex",
                schema: "catalog",
                table: "catalog_attribute_options");
        }
    }
}

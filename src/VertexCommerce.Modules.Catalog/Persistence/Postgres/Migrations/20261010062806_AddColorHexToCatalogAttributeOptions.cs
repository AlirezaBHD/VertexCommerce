using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddColorHexToCatalogAttributeOptions : Migration
    {
        private static readonly Guid ColorAttributeId = new("c0108000-0000-0000-0000-000000000001");

        private static readonly Guid[] ColorOptionIds =
        [
            new("c0108000-0000-0000-0000-000000000011"),
            new("c0108000-0000-0000-0000-000000000012"),
            new("c0108000-0000-0000-0000-000000000013"),
            new("c0108000-0000-0000-0000-000000000014"),
            new("c0108000-0000-0000-0000-000000000015"),
            new("c0108000-0000-0000-0000-000000000016"),
            new("c0108000-0000-0000-0000-000000000017"),
            new("c0108000-0000-0000-0000-000000000018")
        ];

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

            var seedTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "catalog_attributes",
                columns: new[] { "id", "code", "default_name", "type", "created_at", "updated_at", "IsDeleted" },
                values: new object[] { ColorAttributeId, "color", "رنگ", null, seedTimestamp, null, false });

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "catalog_attribute_options",
                columns: new[] { "id", "attribute_id", "code", "default_name", "media_path", "color_hex", "created_at", "updated_at", "IsDeleted" },
                values: new object[,]
                {
                    { ColorOptionIds[0], ColorAttributeId, "black", "Black", null, "#000000", seedTimestamp, null, false },
                    { ColorOptionIds[1], ColorAttributeId, "white", "White", null, "#FFFFFF", seedTimestamp, null, false },
                    { ColorOptionIds[2], ColorAttributeId, "red", "Red", null, "#FF0000", seedTimestamp, null, false },
                    { ColorOptionIds[3], ColorAttributeId, "blue", "Blue", null, "#0000FF", seedTimestamp, null, false },
                    { ColorOptionIds[4], ColorAttributeId, "green", "Green", null, "#008000", seedTimestamp, null, false },
                    { ColorOptionIds[5], ColorAttributeId, "yellow", "Yellow", null, "#FFFF00", seedTimestamp, null, false },
                    { ColorOptionIds[6], ColorAttributeId, "gray", "Gray", null, "#808080", seedTimestamp, null, false },
                    { ColorOptionIds[7], ColorAttributeId, "brown", "Brown", null, "#8B4513", seedTimestamp, null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "catalog",
                table: "catalog_attribute_options",
                keyColumn: "attribute_id",
                keyValue: ColorAttributeId);

            migrationBuilder.DeleteData(
                schema: "catalog",
                table: "catalog_attributes",
                keyColumn: "id",
                keyValue: ColorAttributeId);

            migrationBuilder.DropColumn(
                name: "color_hex",
                schema: "catalog",
                table: "catalog_attribute_options");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMediaFileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "catalog",
                table: "media_files",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                schema: "catalog",
                table: "media_files",
                newName: "size_bytes");

            migrationBuilder.RenameColumn(
                name: "RelativePath",
                schema: "catalog",
                table: "media_files",
                newName: "relative_path");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                schema: "catalog",
                table: "media_files",
                newName: "original_file_name");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                schema: "catalog",
                table: "media_files",
                newName: "content_type");

            migrationBuilder.RenameColumn(
                name: "ConfirmedAt",
                schema: "catalog",
                table: "media_files",
                newName: "confirmed_at");

            migrationBuilder.AlterColumn<string>(
                name: "relative_path",
                schema: "catalog",
                table: "media_files",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "original_file_name",
                schema: "catalog",
                table: "media_files",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                schema: "catalog",
                table: "media_files",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                schema: "catalog",
                table: "media_files",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "size_bytes",
                schema: "catalog",
                table: "media_files",
                newName: "SizeBytes");

            migrationBuilder.RenameColumn(
                name: "relative_path",
                schema: "catalog",
                table: "media_files",
                newName: "RelativePath");

            migrationBuilder.RenameColumn(
                name: "original_file_name",
                schema: "catalog",
                table: "media_files",
                newName: "OriginalFileName");

            migrationBuilder.RenameColumn(
                name: "content_type",
                schema: "catalog",
                table: "media_files",
                newName: "ContentType");

            migrationBuilder.RenameColumn(
                name: "confirmed_at",
                schema: "catalog",
                table: "media_files",
                newName: "ConfirmedAt");

            migrationBuilder.AlterColumn<string>(
                name: "RelativePath",
                schema: "catalog",
                table: "media_files",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalFileName",
                schema: "catalog",
                table: "media_files",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "catalog",
                table: "media_files",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}

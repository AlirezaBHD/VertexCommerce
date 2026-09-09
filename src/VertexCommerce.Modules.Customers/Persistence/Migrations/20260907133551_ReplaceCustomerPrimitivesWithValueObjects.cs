using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VertexCommerce.Modules.Customers.Persistence.Migrations
{
    /// <summary>
    /// Moves the customer aggregate onto value objects.
    /// </summary>
    /// <remarks>
    /// No column is added, renamed, retyped or resized: every value object was mapped explicitly
    /// onto the column it replaced, so this migration carries no data change for them.
    /// <para>
    /// Two things do change. <c>UserId</c> is finally aligned with the model, which has declared it
    /// optional for some time without a migration to match. And the scaffolder wanted to drop the
    /// <c>PhoneNumber</c> and <c>PostalCode</c> indexes, because EF Core cannot express an index
    /// over a property of a complex type until EF Core 11 (dotnet/efcore#31246). Those drops are
    /// deliberately omitted: the columns still exist under the same names, the indexes are still
    /// needed by phone-number lookup and postal-code search, and because the model never mentions
    /// them the scaffolder will not try to drop them again. Once the project is on a version that
    /// can declare them, move them back into the entity configurations and delete this note.
    /// </para>
    /// </remarks>
    public partial class ReplaceCustomerPrimitivesWithValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "customers",
                table: "Customers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "customers",
                table: "Customers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}

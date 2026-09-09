using FluentAssertions;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminAddAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminEditAddress;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.EditAddress;
using VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;
using VertexCommerce.Modules.Customers.Features.Customers.Commands.UpdateCustomer;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Domain.Schema;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Persistence;

/// <summary>
/// Proves the chain that makes a spec the single source of truth:
/// value object spec → EF Core column → FluentValidation rule.
/// If any link drifts, one of these fails.
/// </summary>
public class CustomerSchemaGuardTests
{
    private readonly CustomersDbContext _dbContext;

    public CustomerSchemaGuardTests()
    {
        var options = new DbContextOptionsBuilder<CustomersDbContext>()
            .UseNpgsql("Host=localhost;Database=dummy")
            .Options;

        _dbContext = new CustomersDbContext(options);
    }

    public static TheoryData<Type, string, StringFieldSchema> MappedColumns => new()
    {
        { typeof(Customer), "PhoneNumber", PhoneNumber.Schema },
        { typeof(Customer), "FirstName", FirstName.Schema },
        { typeof(Customer), "LastName", LastName.Schema },
        { typeof(CustomerAddress), "Province", Province.Schema },
        { typeof(CustomerAddress), "City", City.Schema },
        { typeof(CustomerAddress), "PostalAddress", PostalAddress.Schema },
        { typeof(CustomerAddress), "PostalCode", PostalCode.Schema },
        { typeof(CustomerAddress), "Label", AddressLabel.Schema }
    };
    [Theory]
    [MemberData(nameof(MappedColumns))]
    public void Column_shape_comes_from_the_spec_of_the_value_object_that_owns_it(
        Type entityType, string columnName, StringFieldSchema spec)
    {
        var column = FindColumn(entityType, columnName);

        column.GetMaxLength().Should().Be(spec.MaxLength,
            $"column '{columnName}' must be sized by the spec that also guards the domain");
        column.IsUnicode().Should().Be(spec.IsUnicode);
        column.IsFixedLength().Should().Be(spec.IsFixedLength);
    }
    [Fact]
    public void Every_string_column_declares_a_max_length()
    {
        // Flattened properties are used deliberately: value objects are mapped as complex types,
        // and GetProperties() would skip them, leaving this guard asserting nothing.
        var offenders = _dbContext.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetFlattenedProperties()
                .Where(p => p.ClrType == typeof(string) && p.GetMaxLength() is null)
                .Select(p => $"{entity.ShortName()}.{p.Name}"))
            .ToList();

        offenders.Should().BeEmpty("every string column must be sized by a spec");
    }

    [Fact]
    public void Value_objects_map_onto_the_columns_they_replaced()
    {
        // Introducing the value objects must not move any data, so the original column names and
        // the original storage types have to survive untouched.
        ColumnTypes(typeof(Customer)).Should().Contain(new Dictionary<string, string?>
        {
            ["PhoneNumber"] = "character varying(20)",
            ["FirstName"] = "character varying(100)",
            ["LastName"] = "character varying(100)"
        });

        ColumnTypes(typeof(CustomerAddress)).Should().Contain(new Dictionary<string, string?>
        {
            ["Province"] = "character varying(100)",
            ["City"] = "character varying(100)",
            ["PostalAddress"] = "character varying(500)",
            ["PostalCode"] = "character varying(10)",
            ["Label"] = "character varying(50)",
            ["Latitude"] = "numeric(9,6)",
            ["Longitude"] = "numeric(9,6)"
        });
    }

    [Theory]
    [InlineData("IX_Customers_PhoneNumber")]
    [InlineData("IX_CustomerAddresses_PostalCode")]
    public void Indexes_that_the_model_cannot_declare_still_survive_in_the_schema(string indexName)
    {
        // These two sit on columns owned by a complex type, which EF cannot index until EF Core 11,
        // so they live in the migrations alone. Without this guard a future scaffolded migration
        // could quietly drop them and nothing else would notice.
        var script = _dbContext.GetService<IMigrator>().GenerateScript();

        script.Should().Contain(indexName,
            "the index must still be created by the migration history");

        script.Should().NotContain($"DROP INDEX customers.\"{indexName}\"",
            "no migration may drop it while the model is unable to recreate it");
    }

    private IProperty FindColumn(Type entityType, string columnName)
    {
        var entity = _dbContext.Model.FindEntityType(entityType);
        entity.Should().NotBeNull();

        var column = entity!.GetFlattenedProperties()
            .SingleOrDefault(p => p.GetColumnName() == columnName);

        column.Should().NotBeNull($"the model must map a column named '{columnName}'");
        return column!;
    }

    private Dictionary<string, string?> ColumnTypes(Type entityType) =>
        _dbContext.Model.FindEntityType(entityType)!
            .GetFlattenedProperties()
            .ToDictionary(p => p.GetColumnName(), p => (string?)p.GetColumnType());
}

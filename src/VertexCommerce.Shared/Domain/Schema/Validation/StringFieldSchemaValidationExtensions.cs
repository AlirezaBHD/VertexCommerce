using FluentValidation;
using VertexCommerce.Shared.Domain.Schema;

namespace FluentValidation;

public static class StringFieldSchemaValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> HasSchema<T>(
        this IRuleBuilder<T, string?> rule,
        StringFieldSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        IRuleBuilderOptions<T, string?> options = schema.AllowEmpty
            ? rule.MaximumLength(schema.MaxLength)
            : rule.NotEmpty().MaximumLength(schema.MaxLength);

        if (schema.MinLength > 0)
        {
            options = options.MinimumLength(schema.MinLength);
        }

        if (schema.Pattern is not null)
        {
            options = options.Matches(schema.Pattern);
        }

        return options;
    }

    public static IRuleBuilderOptions<T, string?> MustInstantiate<T, TValueObject>(
        this IRuleBuilder<T, string?> rule,
        Func<string?, TValueObject> factory)
    {
        return (IRuleBuilderOptions<T, string?>)rule.Custom((value, context) =>
        {
            try
            {
                factory(value);
            }
            catch (VertexCommerce.Shared.Exceptions.DomainValidationException ex)
            {
                context.AddFailure(context.PropertyPath, ex.Message);
            }
        });
    }
}

using FluentValidation;
using VertexCommerce.Shared.Domain.Schema;

namespace FluentValidation;

public static class StringFieldSpecValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> HasSpec<T>(
        this IRuleBuilder<T, string?> rule,
        StringFieldSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);

        IRuleBuilderOptions<T, string?> options = spec.AllowEmpty
            ? rule.MaximumLength(spec.MaxLength)
            : rule.NotEmpty().MaximumLength(spec.MaxLength);

        if (spec.MinLength > 0)
        {
            options = options.MinimumLength(spec.MinLength);
        }

        if (spec.Pattern is not null)
        {
            options = options.Matches(spec.Pattern);
        }

        return options;
    }
}

using FluentValidation;
using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;

public sealed class CreateOrUpdateBannerCommandValidator : AbstractValidator<CreateOrUpdateBannerCommand>
{
    public CreateOrUpdateBannerCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Target).NotNull().WithMessage("Target is required.");

        When(x => x.Target?.Type == TargetType.None, () =>
        {
            RuleFor(x => x.Target.ProductId).Null().WithMessage("ProductId must not be set when type is None.");
            RuleFor(x => x.Target.CategoryId).Null().WithMessage("CategoryId must not be set when type is None.");
            RuleFor(x => x.Target.InternalPath).Null().WithMessage("InternalPath must not be set when type is None.");
            RuleFor(x => x.Target.ExternalUrl).Null().WithMessage("ExternalUrl must not be set when type is None.");
        });

        When(x => x.Target?.Type == TargetType.Product, () =>
        {
            RuleFor(x => x.Target.ProductSlugSnapshot)
                .NotEmpty().WithMessage("ProductSlugSnapshot is required when targeting a Product.")
                .MaximumLength(500);

            RuleFor(x => x.Target.ProductId)
                .NotEmpty().WithMessage("ProductId is required when targeting a Product.");

            RuleFor(x => x.Target.InternalPath).Null().WithMessage("InternalPath must not be used with Product target.");
            RuleFor(x => x.Target.ExternalUrl).Null().WithMessage("ExternalUrl must not be used with Product target.");
        });

        When(x => x.Target?.Type == TargetType.Category, () =>
        {
            RuleFor(x => x.Target.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required when targeting a Category.");

            RuleFor(x => x.Target.CategoryTitleSnapshot)
                .NotEmpty().WithMessage("CategoryTitleSnapshot is required when targeting a Category.")
                .MaximumLength(200);

            RuleFor(x => x.Target.CategorySlugSnapshot)
                .NotEmpty().WithMessage("CategorySlugSnapshot is required when targeting a Category.")
                .MaximumLength(500);

            RuleFor(x => x.Target.InternalPath).Null().WithMessage("InternalPath must not be used with Category target.");
            RuleFor(x => x.Target.ExternalUrl).Null().WithMessage("ExternalUrl must not be used with Category target.");
        });

        When(x => x.Target?.Type == TargetType.InternalPath, () =>
        {
            RuleFor(x => x.Target.InternalPath)
                .NotEmpty().WithMessage("InternalPath is required.")
                .Must(path => path != null && path.StartsWith('/'))
                .WithMessage("InternalPath must start with '/'.")
                .Must(path => path != null && path.StartsWith("//"))
                .WithMessage("InternalPath must not start with '//'.")
                .Must(path =>
                {
                    if (string.IsNullOrEmpty(path)) return true;
                    return !Uri.TryCreate(path, UriKind.Absolute, out var uri) || !uri.Scheme.StartsWith("http");
                })
                .WithMessage("InternalPath must not be an absolute URL.")
                .MaximumLength(500);

            RuleFor(x => x.Target.ProductId).Null().WithMessage("ProductId must not be set for InternalPath target.");
            RuleFor(x => x.Target.CategoryId).Null().WithMessage("CategoryId must not be set for InternalPath target.");
            RuleFor(x => x.Target.ExternalUrl).Null().WithMessage("ExternalUrl must not be used with InternalPath target.");
        });

        When(x => x.Target?.Type == TargetType.ExternalUrl, () =>
        {
            RuleFor(x => x.Target.ExternalUrl)
                .NotEmpty().WithMessage("ExternalUrl is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                             && (uri.Scheme == "http" || uri.Scheme == "https"))
                .WithMessage("ExternalUrl must be a valid http or https URL.")
                .MaximumLength(2000);

            RuleFor(x => x.Target.ProductId).Null().WithMessage("ProductId must not be set for ExternalUrl target.");
            RuleFor(x => x.Target.CategoryId).Null().WithMessage("CategoryId must not be set for ExternalUrl target.");
            RuleFor(x => x.Target.InternalPath).Null().WithMessage("InternalPath must not be used with ExternalUrl target.");
        });
    }
}

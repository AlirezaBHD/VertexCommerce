using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteBanner;

public sealed record DeleteBannerCommand(Guid Id) : ICommand;

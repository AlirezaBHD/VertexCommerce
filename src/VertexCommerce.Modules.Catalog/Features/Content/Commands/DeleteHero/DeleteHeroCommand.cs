using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteHero;

public sealed record DeleteHeroCommand(Guid Id) : ICommand;

using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.SetActiveHero;

public sealed record SetActiveHeroCommand(Guid Id) : ICommand;

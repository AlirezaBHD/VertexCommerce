using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;

public sealed record UpdateContactRequest(
    string Title,
    string Subtitle,
    string Description,
    string Email,
    string Phone,
    string Address,
    string? WorkingHours,
    string? MapEmbedUrl,
    List<SocialLinkDto>? SocialLinks);

public sealed record UpdateContactCommand(
    string Title,
    string Subtitle,
    string Description,
    string Email,
    string Phone,
    string Address,
    string? WorkingHours,
    string? MapEmbedUrl,
    List<SocialLinkDto>? SocialLinks) : ICommand;

public sealed record SocialLinkDto(string Platform, string Label, string Url, string Icon);

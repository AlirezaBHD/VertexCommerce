using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record UpdateAboutRequest(
    string Title,
    string Subtitle,
    string Description,
    string? Mission,
    string? Vision,
    List<AboutValueItemDto>? Values,
    List<AboutStatItemDto>? Stats,
    List<AboutTeamMemberDto>? Team);

public sealed record UpdateAboutCommand(
    string Title,
    string Subtitle,
    string Description,
    string? Mission,
    string? Vision,
    List<AboutValueItemDto>? Values,
    List<AboutStatItemDto>? Stats,
    List<AboutTeamMemberDto>? Team) : ICommand;

public sealed record AboutValueItemDto(string Icon, string Title, string Description);
public sealed record AboutStatItemDto(string Label, string Value, string? Suffix);
public sealed record AboutTeamMemberDto(string Name, string Role, string? Bio, string? ImagePath);

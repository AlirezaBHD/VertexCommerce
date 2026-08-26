using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

internal sealed class UpdateAboutCommandHandler(IContentRepository contentRepository)
    : ICommandHandler<UpdateAboutCommand>
{
    public async Task<Result> Handle(UpdateAboutCommand command, CancellationToken ct)
    {
        var existing = await contentRepository.GetAboutAsync(ct) ?? AboutDocument.CreateDefault();

        var doc = new AboutDocument
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Hero = new AboutHeroSection
            {
                Badge = command.Hero?.Badge ?? existing.Hero.Badge,
                Title = command.Hero?.Title ?? existing.Hero.Title,
                Subtitle = command.Hero?.Subtitle ?? existing.Hero.Subtitle,
                ButtonText = command.Hero?.ButtonText ?? existing.Hero.ButtonText,
                ImagePath = command.Hero?.ImagePath ?? existing.Hero.ImagePath,
                ShowCat = command.Hero?.ShowCat ?? existing.Hero.ShowCat,
                CatImagePath = command.Hero?.CatImagePath ?? existing.Hero.CatImagePath,
            },
            Commitments = new AboutCommitmentsSection
            {
                Badge = command.Commitments?.Badge ?? existing.Commitments.Badge,
                Title = command.Commitments?.Title ?? existing.Commitments.Title,
                Subtitle = command.Commitments?.Subtitle ?? existing.Commitments.Subtitle,
                Items = (command.Commitments?.Items ?? new()).Select(i => new AboutCommitmentItem
                {
                    Title = i.Title ?? string.Empty,
                    Description = i.Description ?? string.Empty,
                    Badge = i.Badge ?? string.Empty,
                    Icon = i.Icon ?? string.Empty,
                }).ToList(),
            },
            Quality = new AboutQualitySection
            {
                Badge = command.Quality?.Badge ?? existing.Quality.Badge,
                Title = command.Quality?.Title ?? existing.Quality.Title,
                Paragraphs = command.Quality?.Paragraphs ?? existing.Quality.Paragraphs,
                Features = (command.Quality?.Features ?? new()).Select(f => new AboutQualityFeatureItem
                {
                    Title = f.Title ?? string.Empty,
                    Description = f.Description ?? string.Empty,
                }).ToList(),
                ImagePath = command.Quality?.ImagePath ?? existing.Quality.ImagePath,
                ImageBadgeTitle = command.Quality?.ImageBadgeTitle ?? existing.Quality.ImageBadgeTitle,
                ImageBadgeSubtitle = command.Quality?.ImageBadgeSubtitle ?? existing.Quality.ImageBadgeSubtitle,
            },
            Process = new AboutProcessSection
            {
                Badge = command.Process?.Badge ?? existing.Process.Badge,
                Title = command.Process?.Title ?? existing.Process.Title,
                Subtitle = command.Process?.Subtitle ?? existing.Process.Subtitle,
                Steps = (command.Process?.Steps ?? new()).Select(s => new AboutProcessStepItem
                {
                    Title = s.Title ?? string.Empty,
                    Description = s.Description ?? string.Empty,
                    Icon = s.Icon ?? string.Empty,
                }).ToList(),
            },
            Story = new AboutStorySection
            {
                Badge = command.Story?.Badge ?? existing.Story.Badge,
                Title = command.Story?.Title ?? existing.Story.Title,
                Paragraphs = command.Story?.Paragraphs ?? existing.Story.Paragraphs,
                ImagePath = command.Story?.ImagePath ?? existing.Story.ImagePath,
                ImageBadge = command.Story?.ImageBadge ?? existing.Story.ImageBadge,
                SupportText = command.Story?.SupportText ?? existing.Story.SupportText,
            },
            Cta = new AboutCtaSection
            {
                Title = command.Cta?.Title ?? existing.Cta.Title,
                Subtitle = command.Cta?.Subtitle ?? existing.Cta.Subtitle,
                ButtonText = command.Cta?.ButtonText ?? existing.Cta.ButtonText,
                ButtonLink = command.Cta?.ButtonLink ?? existing.Cta.ButtonLink,
            },
            UpdatedAt = DateTime.UtcNow
        };

        await contentRepository.UpsertAboutAsync(doc, ct);
        return Result.Success();
    }
}

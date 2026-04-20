
namespace AleonAPI.Features.Sites.Commands.CreateSite;

public class CreateSiteHandler(ApplicationDbContext context)
    : IRequestHandler<CreateSiteCommand, PrivateSiteResponse>
{
    public async Task<PrivateSiteResponse> Handle(CreateSiteCommand request, CancellationToken ct)
    {
        var site = new Site
        {
            Name = request.Name,
            Location = request.Location,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Description = request.Description,
            PublicNarrative = request.PublicNarrative,
            AeonNarrative = request.AeonNarrative
        };

        context.Sites.Add(site);
        await context.SaveChangesAsync(ct);

        return new PrivateSiteResponse
        {
            Id = site.Id,
            Name = site.Name,
            Location = site.Location,
            Latitude = site.Latitude,
            Longitude = site.Longitude,
            Description = site.Description,
            PublicNarrative = site.PublicNarrative,
            AeonNarrative = site.AeonNarrative
        };
    }
}

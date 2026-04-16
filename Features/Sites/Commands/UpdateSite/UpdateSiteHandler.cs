using MediatR;

namespace AleonAPI.Features.Sites.Commands.UpdateSite;

public class UpdateSiteHandler(ApplicationDbContext context)
    : IRequestHandler<UpdateSiteCommand, PrivateSiteResponse?>
{
    public async Task<PrivateSiteResponse?> Handle(UpdateSiteCommand request, CancellationToken ct)
    {
        var site = await context.Sites.FindAsync(request.SiteId, ct);
        if (site == null) return null;

        site.Name = request.Name;
        site.Location = request.Location;
        site.Latitude = request.Latitude;
        site.Longitude = request.Longitude;
        site.Description = request.Description;
        site.PublicNarrative = request.PublicNarrative;
        site.AeonNarrative = request.AeonNarrative;

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

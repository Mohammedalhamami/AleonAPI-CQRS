using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Features.Sites.Queries.GetPublicSiteById;

public class GetPublicSiteByIdHandler(ApplicationDbContext context)
    : IRequestHandler<GetPublicSiteByIdQuery, PublicSiteResponse?>
{
    public async Task<PublicSiteResponse?> Handle(GetPublicSiteByIdQuery request, CancellationToken ct)
    {
        return await context.Sites
            .AsNoTracking()
            .Where(s => s.Id == request.SiteId)
            .Select(s => new PublicSiteResponse
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative
            }).FirstOrDefaultAsync(ct);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Features.Sites.Queries.GetAllPublicSites;

public class GetAllPublicSitesHandler(ApplicationDbContext context)
    : IRequestHandler<GetAllPublicSitesQuery, IEnumerable<PublicSiteResponse>>
{
    public async Task<IEnumerable<PublicSiteResponse>> Handle(GetAllPublicSitesQuery request, CancellationToken ct)
    {
        return await context.Sites
            .AsNoTracking()
            .Select(s => new PublicSiteResponse
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative
            }).ToListAsync(ct);
    }
}

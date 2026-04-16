using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Features.Sites.Queries.GetAllPrivateSites;

public class GetAllPrivateSitesHandler(ApplicationDbContext context)
    : IRequestHandler<GetAllPrivateSitesQuery, IEnumerable<PrivateSiteResponse>>
{
    public async Task<IEnumerable<PrivateSiteResponse>> Handle(GetAllPrivateSitesQuery request, CancellationToken ct)
    {
        return await context.Sites
            .AsNoTracking()
            .Select(s => new PrivateSiteResponse
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Latitude = s.Latitude,
                Longitude = s.Longitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative,
                AeonNarrative = s.AeonNarrative
            }).ToListAsync(ct);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Features.Sites.Queries.GetPrivateSiteById;

public class GetPrivateSiteByIdHandler(ApplicationDbContext context)
    : IRequestHandler<GetPrivateSiteByIdQuery, PrivateSiteResponse?>
{
    public async Task<PrivateSiteResponse?> Handle(GetPrivateSiteByIdQuery request, CancellationToken ct)
    {
        return await context.Sites
            .AsNoTracking()
            .Where(s => s.Id == request.SiteId)
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
            }).FirstOrDefaultAsync(ct);
    }
}

using System;
using AleonAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Services;

public class SiteService(ApplicationDbContext context) : ISiteService
{

        public async Task<IEnumerable<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct)
        {
            // Implementation to retrieve all sites from the database and map them to PublicSiteResponse
            return await context
            .Sites
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

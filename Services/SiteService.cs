using System;
using System.Reflection.Metadata.Ecma335;
using AleonAPI.Models.Request;
using AleonAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Services;

public class SiteService(ApplicationDbContext context) : ISiteService
{
    public async Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct)
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

    public async Task<bool> DeleteSiteAsync(int siteId, CancellationToken ct)
    {
        var site = await context.Sites.FindAsync(siteId, ct);

        if (site == null)
        {
            return false;
        }

        context.Sites.Remove(site);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct)
    {
        return await context
           .Sites
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

    public async Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int siteId, CancellationToken ct)
    {
        return await context
             .Sites
             .AsNoTracking()
             .Where(s => s.Id == siteId)
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

    public async Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int siteId, CancellationToken ct)
    {
        return await context
             .Sites
             .AsNoTracking()
             .Where(s => s.Id == siteId)
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

    public async Task<PrivateSiteResponse?> UpdateSiteAsync(int siteId, UpdateSiteRequest request, CancellationToken ct)
    {
        var site = await context.Sites.FindAsync(siteId, ct);

        if (site == null)
        {
            return null;
        }

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

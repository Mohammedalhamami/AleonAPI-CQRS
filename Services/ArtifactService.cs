using AleonAPI.Enums;
using AleonAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Services;

public class ArtifactService(ApplicationDbContext context) : IArtifactService
{
    public async Task<List<PrivateArtifactResponse>> GetAllPrivateArtifactsAsync(CancellationToken ct)
    {
        var artifacts = await context.Artifacts
            .AsNoTracking()
            .Include(a => a.Site)
            .Include(a => a.MediaFiles)
            .Select(a => new PrivateArtifactResponse
            {
                Id = a.Id,
                Name = a.Name,
                CatalogNumber = a.CatalogNumber,
                PublicNarrative = a.PublicNarrative,
                DateDiscovered = a.DateDiscovered,
                Description = a.Description,
                SiteId = a.SiteId,
                Type = a.Type,
                SiteName = a.Site != null ? a.Site.Name : string.Empty,
                PrimaryImageUrl = a.MediaFiles
                .Where(mf => mf.IsPrimary)
                .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
                .FirstOrDefault()
            }).ToListAsync(ct);


        return artifacts;
    }

    public async Task<List<PublicArtifactResponse>> GetAllPublicArtifactsAsync(CancellationToken ct)
    {
        var artifacts = await context.Artifacts
            .AsNoTracking()
            .Include(a => a.Site)
            .Include(a => a.MediaFiles)
            .Select(a => new PublicArtifactResponse
            {
                Id = a.Id,
                Name = a.Name,
                CatalogNumber = a.CatalogNumber,
                PublicNarrative = a.PublicNarrative,
                DateDiscovered = a.DateDiscovered,
                Type = a.Type,
                SiteName = a.Site != null ? a.Site.Name : string.Empty,
                PrimaryImageUrl = a.MediaFiles
                .Where(mf => mf.IsPrimary)
                .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
                .FirstOrDefault()
            }).ToListAsync(ct);


        return artifacts;
    }

    public async Task<List<PublicArtifactResponse>?> GetPublicArtifactsBySiteIdAsync(int siteId, CancellationToken ct)
    {

        // Confirm the site exists
        if (!await CheckSiteExistsAsync(siteId, ct)) return null;

        var artifacts = await context.Artifacts
           .AsNoTracking()
           .Include(a => a.Site)
           .Include(a => a.MediaFiles)
           .Where(a => a.SiteId == siteId)
           .Select(a => new PublicArtifactResponse
           {
               Id = a.Id,
               Name = a.Name,
               CatalogNumber = a.CatalogNumber,
               PublicNarrative = a.PublicNarrative,
               DateDiscovered = a.DateDiscovered,
               Type = a.Type,
               SiteId = a.SiteId,
               SiteName = a.Site != null ? a.Site.Name : string.Empty,
               PrimaryImageUrl = a.MediaFiles
               .Where(mf => mf.IsPrimary)
               .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
               .FirstOrDefault()
           }).ToListAsync(ct);


        return artifacts;
    }

    public async Task<List<PrivateArtifactResponse>?> GetPrivateArtifactsBySiteIdAsync(int siteId, CancellationToken ct)
    {

        // Confirm the site exists
        if (!await CheckSiteExistsAsync(siteId, ct)) return null;

        var artifacts = await context.Artifacts
            .AsNoTracking()
            .Include(a => a.Site)
            .Include(a => a.MediaFiles)
            .Where(a => a.SiteId == siteId)
            .Select(a => new PrivateArtifactResponse
            {
                Id = a.Id,
                Name = a.Name,
                CatalogNumber = a.CatalogNumber,
                PublicNarrative = a.PublicNarrative,
                DateDiscovered = a.DateDiscovered,
                SiteId = a.SiteId,
                Type = a.Type,
                Description = a.Description,
                SiteName = a.Site != null ? a.Site.Name : string.Empty,
                PrimaryImageUrl = a.MediaFiles
                .Where(mf => mf.IsPrimary)
                .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
                .FirstOrDefault(),
                CatalogRecordCount = a.CatalogRecords.Count,
            }).ToListAsync(ct);


        return artifacts;
    }

    public async Task<PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct)
    {

        // Confirm the site exists
        var site = await context.Sites.AsNoTracking()
        .FirstOrDefaultAsync(s => s.Id == request.SiteId, ct);

        if (site == null) return new PrivateArtifactResponse();

        // Validate the artifact type string
        if (!Enum.TryParse<ArtifactType>(request.Type, true, out var artifactType))
        {
            throw new ArgumentException($"Invalid artifact type '{request.Type}'. " +
                $"Allowed values are: {string.Join(", ", Enum.GetNames(typeof(ArtifactType)))}");
        }

        var artifact = new Artifact
        {
            Name = request.Name,
            CatalogNumber = request.CatalogNumber,
            PublicNarrative = request.PublicNarrative,
            DateDiscovered = request.DateDiscovered,
            Type = artifactType.ToString(),
            SiteId = request.SiteId,
            Description = request.Description,
        };

        context.Artifacts.Add(artifact);
        await context.SaveChangesAsync(ct);

        return new PrivateArtifactResponse
        {
            Id = artifact.Id,
            Name = artifact.Name,
            CatalogNumber = artifact.CatalogNumber,
            PublicNarrative = artifact.PublicNarrative,
            DateDiscovered = artifact.DateDiscovered,
            Type = artifact.Type,
            SiteId = artifact.SiteId,
            Description = artifact.Description,
            SiteName = site.Name,
            PrimaryImageUrl = artifact.MediaFiles
            .Where(mf => mf.IsPrimary)
            .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
            .FirstOrDefault(),
            CatalogRecordCount = artifact.CatalogRecords.Count,
        };
    }


    public async Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int artifactId, CancellationToken ct)
    {
        var artifact = await context.Artifacts
            .AsNoTracking()
            .Include(a => a.Site)
            .Include(a => a.MediaFiles)
            .FirstOrDefaultAsync(a => a.Id == artifactId, ct);

        if (artifact == null) return null;

        return new PublicArtifactResponse
        {
            Id = artifact.Id,
            Name = artifact.Name,
            CatalogNumber = artifact.CatalogNumber,
            PublicNarrative = artifact.PublicNarrative,
            DateDiscovered = artifact.DateDiscovered,
            Type = artifact.Type,
            SiteId = artifact.SiteId,
            SiteName = artifact.Site != null ? artifact.Site.Name : string.Empty,
            PrimaryImageUrl = artifact.MediaFiles
            .Where(mf => mf.IsPrimary)
            .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
            .FirstOrDefault()
        };
    }

    public async Task<PrivateArtifactResponse?> GetPrivateArtifactByIdAsync(int artifactId, CancellationToken ct)
    {
        return await context.Artifacts
            .AsNoTracking()
            .Include(a => a.Site)
            .Include(a => a.MediaFiles)
            .Where(a => a.Id == artifactId).Select(a => new PrivateArtifactResponse
            {
                Id = a.Id,
                Name = a.Name,
                CatalogNumber = a.CatalogNumber,
                PublicNarrative = a.PublicNarrative,
                DateDiscovered = a.DateDiscovered,
                Type = a.Type,
                SiteId = a.SiteId,
                Description = a.Description,
                SiteName = a.Site != null ? a.Site.Name : string.Empty,
                PrimaryImageUrl = a.MediaFiles
                .Where(mf => mf.IsPrimary)
                .Select(mf => $"/api/public/artifacts/images/{mf.Id}")
                .FirstOrDefault(),
                CatalogRecordCount = a.CatalogRecords.Count,
            }).FirstOrDefaultAsync(ct);

    }


    private async Task<bool> CheckSiteExistsAsync(int siteId, CancellationToken ct)
    {
        // Confirm the site exists
        return await context.Sites.AsNoTracking().AnyAsync(s => s.Id == siteId, ct);
    }

}

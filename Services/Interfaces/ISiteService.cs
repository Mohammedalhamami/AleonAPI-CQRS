using System;
using AleonAPI.Models.Request;

namespace AleonAPI.Services.Interfaces;

public interface ISiteService
{

   Task<IEnumerable<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct);
    Task<IEnumerable<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct);

   Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int siteId, CancellationToken ct);

   Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int siteId, CancellationToken ct);
   Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct);
   Task<PrivateSiteResponse?> UpdateSiteAsync(int siteId, UpdateSiteRequest request, CancellationToken ct);

   Task<bool> DeleteSiteAsync(int siteId, CancellationToken ct);
}

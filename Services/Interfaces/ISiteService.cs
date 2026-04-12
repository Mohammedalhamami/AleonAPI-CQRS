using System;

namespace AleonAPI.Services.Interfaces;

public interface ISiteService
{

   Task<IEnumerable<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct);

}

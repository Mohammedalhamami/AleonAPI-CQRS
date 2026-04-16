using MediatR;

namespace AleonAPI.Features.Sites.Queries.GetAllPublicSites;

public record GetAllPublicSitesQuery : IRequest<IEnumerable<PublicSiteResponse>>;

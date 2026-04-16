using MediatR;

namespace AleonAPI.Features.Sites.Queries.GetPublicSiteById;

public record GetPublicSiteByIdQuery(int SiteId) : IRequest<PublicSiteResponse?>;

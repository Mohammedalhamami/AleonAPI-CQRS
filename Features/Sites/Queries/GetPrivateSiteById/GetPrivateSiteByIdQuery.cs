using MediatR;

namespace AleonAPI.Features.Sites.Queries.GetPrivateSiteById;

public record GetPrivateSiteByIdQuery(int SiteId) : IRequest<PrivateSiteResponse?>;

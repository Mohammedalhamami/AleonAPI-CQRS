using MediatR;

namespace AleonAPI.Features.Sites.Queries.GetAllPrivateSites;

public record GetAllPrivateSitesQuery : IRequest<IEnumerable<PrivateSiteResponse>>;

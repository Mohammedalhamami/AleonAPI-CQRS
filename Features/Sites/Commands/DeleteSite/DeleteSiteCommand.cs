using MediatR;

namespace AleonAPI.Features.Sites.Commands.DeleteSite;

public record DeleteSiteCommand(int SiteId) : IRequest<bool>;

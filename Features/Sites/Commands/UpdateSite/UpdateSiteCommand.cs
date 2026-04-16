using MediatR;

namespace AleonAPI.Features.Sites.Commands.UpdateSite;

public record UpdateSiteCommand(
    int SiteId,
    string Name,
    string Location,
    double Latitude,
    double Longitude,
    string? Description,
    string? PublicNarrative,
    string? AeonNarrative
) : IRequest<PrivateSiteResponse?>;

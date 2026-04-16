using MediatR;

namespace AleonAPI.Features.Sites.Commands.CreateSite;

public record CreateSiteCommand(
    string Name,
    string Location,
    double Latitude,
    double Longitude,
    string? Description,
    string? PublicNarrative,
    string? AeonNarrative
) : IRequest<PrivateSiteResponse>;

using MediatR;

namespace AleonAPI.Features.ArtifactMediaFiles.Commands.CreateArtifactMediaFile;

public record CreateArtifactMediaFileCommand(
    int ArtifactId,
    IFormFile File,
    bool IsPrimary
) : IRequest<ArtifactMediaFile?>;

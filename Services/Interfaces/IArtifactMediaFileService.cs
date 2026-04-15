using System;

namespace AleonAPI.Services.Interfaces;

public interface IArtifactMediaFileService
{
    Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(
        int artifactId,
        IFormFile file,
        bool isPrimary,
         CancellationToken ct);

}

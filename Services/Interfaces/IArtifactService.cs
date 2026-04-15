using System;

namespace AleonAPI.Services.Interfaces;

public interface IArtifactService
{
    //public
    Task<List<PublicArtifactResponse>> GetAllPublicArtifactsAsync(CancellationToken ct);
    Task<List<PublicArtifactResponse>?> GetPublicArtifactsBySiteIdAsync(int siteId, CancellationToken ct);
    Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int artifactId, CancellationToken ct);


    //private
    Task<List<PrivateArtifactResponse>> GetAllPrivateArtifactsAsync(CancellationToken ct);
    Task<List<PrivateArtifactResponse>?> GetPrivateArtifactsBySiteIdAsync(int siteId, CancellationToken ct);
    Task<PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct);
    Task<PrivateArtifactResponse?> GetPrivateArtifactByIdAsync(int artifactId, CancellationToken ct);

}

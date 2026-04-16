using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Features.ArtifactMediaFiles.Commands.CreateArtifactMediaFile;

public class CreateArtifactMediaFileHandler(ApplicationDbContext context)
    : IRequestHandler<CreateArtifactMediaFileCommand, ArtifactMediaFile?>
{
    public async Task<ArtifactMediaFile?> Handle(CreateArtifactMediaFileCommand request, CancellationToken ct)
    {
        var artifact = await context.Artifacts.FindAsync(request.ArtifactId);
        if (artifact == null) return null;

        if (request.File == null || request.File.Length == 0)
            throw new ArgumentException("File is required and cannot be empty.");

        if (request.IsPrimary)
        {
            var existingPrimary = await context.ArtifactMediaFiles
                .Where(mf => mf.ArtifactId == request.ArtifactId && mf.IsPrimary)
                .ToListAsync(ct);

            foreach (var media in existingPrimary)
                media.IsPrimary = false;
        }

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream, ct);

        var mediaFile = new ArtifactMediaFile
        {
            ArtifactId = request.ArtifactId,
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            Data = memoryStream.ToArray(),
            IsPrimary = request.IsPrimary
        };

        context.ArtifactMediaFiles.Add(mediaFile);
        await context.SaveChangesAsync(ct);
        return mediaFile;
    }
}

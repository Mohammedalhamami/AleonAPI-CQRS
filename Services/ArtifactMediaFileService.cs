using System;
using AleonAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Services;

public class ArtifactMediaFileService(ApplicationDbContext context) : IArtifactMediaFileService
{
    public async Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(int artifactId, IFormFile file, bool isPrimary, CancellationToken ct)
    {
        //check if artifact exists
        var artifact = await context.Artifacts.FindAsync(artifactId);
        if (artifact == null)
        {
            return null;
        }

        //validate file
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required and cannot be empty.");
        }

        //check if primary media, then but it ahead of them 
        if (isPrimary)
        {
            var mediaPrimaryList = await context.ArtifactMediaFiles
                .Where(mf => mf.ArtifactId == artifactId && mf.IsPrimary)
                .ToListAsync(ct);

            if (mediaPrimaryList != null)
            {
                foreach (var media in mediaPrimaryList)
                {
                    media.IsPrimary = false;
                }

            }

        }

        //convert IFormFile to byte array
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, ct);
        var data = memoryStream.ToArray();

        var mediaFile = new ArtifactMediaFile
        {
            ArtifactId = artifactId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Data = data,
            IsPrimary = isPrimary
        };

        context.ArtifactMediaFiles.Add(mediaFile);
        await context.SaveChangesAsync(ct);


        return mediaFile;
    }
}

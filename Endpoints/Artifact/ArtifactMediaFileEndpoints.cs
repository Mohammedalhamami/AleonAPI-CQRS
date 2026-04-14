
using AleonAPI.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AleonAPI.Endpoints.Sites;

public static class ArtifactMediaFileEndpoints
{


    public static IEndpointRouteBuilder MapArtifactMediaFileEndpoints(this IEndpointRouteBuilder route)
    {
        //endpoints groups for ArtifactMediaFiles

        var publicGroup = route.MapGroup("api/public/artifacts/images")
            
            .WithSummary("Artifact media file endpoints")
            .WithDescription("Endpoints that expose artifact media file data, accessible only to admin users")
            .WithTags("Artifact Media Files - Public")
            .AllowAnonymous()
            .AddEndpointFilter<ExceptionHandlingFilter>();

        publicGroup.MapGet("/{id:int}", GetArtifactImage)
            .WithName("GetArtifactImage")
            .WithSummary("Get artifact image")
            .WithDescription("Get the image associated with an artifact media file by its ID. Returns the image data as a file response. Accessible only to admin users.")
            .Produces<FileContentHttpResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);



        return publicGroup;

    }



    //then endpoints

    private static async Task<Results<FileContentHttpResult, NotFound>> GetArtifactImage(
        int id,
        ApplicationDbContext dbContext,
        HttpResponse response,
        CancellationToken cancellationToken)
    {

        var mediaFile = await dbContext.ArtifactMediaFiles
        .AsNoTracking()
        .FirstOrDefaultAsync(mf => mf.Id == id);

        if (mediaFile == null || mediaFile.Data.Length == 0)
        {
            return TypedResults.NotFound();
        }
        //optional, set a cache control header to cache the image for 1 day
        response.Headers.CacheControl = "public, max-age=86400";


        return TypedResults.File(mediaFile.Data, mediaFile.ContentType, fileDownloadName: mediaFile.FileName);

    }

}
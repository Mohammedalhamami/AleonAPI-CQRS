
using AleonAPI.Filters;
using AleonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AleonAPI.Endpoints.Artifact;

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


        //private group 
        var privateGroup = route.MapGroup("api/private/artifacts/{artifactId:int}/images")
            .RequireAuthorization()
            .WithSummary("Artifact media file endpoints")
            .WithDescription("Endpoints that expose artifact media file data, accessible only to admin users")
            .WithTags("Artifact Media Files - Private")
            .AddEndpointFilter<ExceptionHandlingFilter>();


        privateGroup.MapPost("", CreateArtifactMediaFile)
            .WithName("CreateArtifactMediaFile")
            .WithSummary("Create artifact media file")
            .WithDescription("Create a new artifact media file for a specific artifact. Accepts an image file and a boolean indicating if it's the primary media file for the artifact. Returns a 201 Created response with the location of the new media file. Accessible only to admin users.")
            .Accepts<IFormFile>("multipart/form-data")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);


        return route;

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


        return TypedResults.File(mediaFile.Data, mediaFile.ContentType);

    }


    private static async Task<Results<Created, NotFound, BadRequest>> CreateArtifactMediaFile(
        int artifactId,
        IFormFile file,
        bool isPrimary,
        IArtifactMediaFileService mediaFileService,
        CancellationToken cancellationToken)
    {

        var mediaFile = await mediaFileService.CreateArtifactMediaFileAsync(artifactId, file, isPrimary, cancellationToken);
        if (mediaFile == null) return TypedResults.NotFound();

        var location = $"/api/public/artifacts/images/{mediaFile.Id}";

        return TypedResults.Created(location);

    }

}
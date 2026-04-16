using AleonAPI.Features.ArtifactMediaFiles.Commands.CreateArtifactMediaFile;
using AleonAPI.Filters;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Endpoints.Artifact;

public static class ArtifactMediaFileEndpoints
{
    public static IEndpointRouteBuilder MapArtifactMediaFileEndpoints(this IEndpointRouteBuilder route)
    {
        var publicGroup = route.MapGroup("api/public/artifacts/images")
            .WithSummary("Artifact media file endpoints")
            .WithDescription("Endpoints that expose artifact media file data")
            .WithTags("Artifact Media Files - Public")
            .AllowAnonymous()
            .AddEndpointFilter<ExceptionHandlingFilter>();

        publicGroup.MapGet("/{id:int}", GetArtifactImage)
            .WithName("GetArtifactImage")
            .WithSummary("Get artifact image")
            .WithDescription("Get the image associated with an artifact media file by its ID.")
            .Produces<FileContentHttpResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        var privateGroup = route.MapGroup("api/private/artifacts/{artifactId:int}/images")
            .RequireAuthorization()
            .WithSummary("Artifact media file endpoints")
            .WithDescription("Endpoints that expose artifact media file data, accessible only to admin users")
            .WithTags("Artifact Media Files - Private")
            .AddEndpointFilter<ExceptionHandlingFilter>();

        privateGroup.MapPost("", CreateArtifactMediaFile)
            .WithName("CreateArtifactMediaFile")
            .WithSummary("Create artifact media file")
            .WithDescription("Upload a new media file for a specific artifact.")
            .Accepts<IFormFile>("multipart/form-data")
            .DisableAntiforgery()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return route;
    }

    private static async Task<Results<FileContentHttpResult, NotFound>> GetArtifactImage(
        int id,
        ApplicationDbContext dbContext,
        HttpResponse response,
        CancellationToken ct)
    {
        var mediaFile = await dbContext.ArtifactMediaFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(mf => mf.Id == id, ct);

        if (mediaFile == null || mediaFile.Data.Length == 0)
            return TypedResults.NotFound();

        response.Headers.CacheControl = "public, max-age=86400";
        return TypedResults.File(mediaFile.Data, mediaFile.ContentType);
    }

    private static async Task<Results<Created, NotFound, BadRequest>> CreateArtifactMediaFile(
        int artifactId,
        IFormFile file,
        bool isPrimary,
        ISender sender,
        CancellationToken ct)
    {
        var mediaFile = await sender.Send(new CreateArtifactMediaFileCommand(artifactId, file, isPrimary), ct);
        if (mediaFile == null) return TypedResults.NotFound();

        return TypedResults.Created($"/api/public/artifacts/images/{mediaFile.Id}");
    }
}
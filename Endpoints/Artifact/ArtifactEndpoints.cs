using AleonAPI.Features.Artifacts.Commands.CreateArtifact;
using AleonAPI.Features.Artifacts.Commands.DeleteArtifact;
using AleonAPI.Features.Artifacts.Commands.UpdateArtifact;
using AleonAPI.Features.Artifacts.Queries.GetAllPrivateArtifacts;
using AleonAPI.Features.Artifacts.Queries.GetAllPublicArtifacts;
using AleonAPI.Features.Artifacts.Queries.GetPrivateArtifactById;
using AleonAPI.Features.Artifacts.Queries.GetPublicArtifactById;
using AleonAPI.Filters;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AleonAPI.Endpoints.Artifact;

public static class ArtifactEndpoints
{
    public static IEndpointRouteBuilder MapArtifactEndpoints(this IEndpointRouteBuilder route)
    {
        var publicGroup = route.MapGroup("api/public/artifacts")
            .WithSummary("Artifact")
            .WithTags("Artifacts - Public")
            .AddEndpointFilter<ExceptionHandlingFilter>()
            .AllowAnonymous();

        publicGroup.MapGet("", GetAllPublicArtifacts)
            .WithName(nameof(GetAllPublicArtifacts))
            .Produces<List<PublicArtifactResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithDescription("Endpoints that expose artifact data");

        publicGroup.MapGet("/{artifactId:int}", GetPublicArtifactById)
            .WithName(nameof(GetPublicArtifactById))
            .WithDescription("Returns a specific artifact by its ID")
            .Produces<PublicArtifactResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a specific artifact by its ID");

        var privateGroup = route.MapGroup("api/private/artifacts")
            .WithSummary("Private artifacts")
            .WithTags("Artifacts - Private")
            .WithDescription("Get all private artifacts")
            .RequireAuthorization()
            .AddEndpointFilter<ExceptionHandlingFilter>();

        privateGroup.MapGet("", GetAllPrivateArtifacts)
            .WithName(nameof(GetAllPrivateArtifacts))
            .WithDescription("Returns all artifacts with their private data")
            .Produces<List<PrivateArtifactResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all private artifacts");

        privateGroup.MapPost("", CreateArtifact)
            .WithName(nameof(CreateArtifact))
            .WithDescription("Creates a new artifact")
            .Accepts<CreateArtifactRequest>("application/json")
            .Produces<PrivateArtifactResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new artifact");

        privateGroup.MapGet("/{artifactId:int}", GetPrivateArtifactById)
            .WithName(nameof(GetPrivateArtifactById))
            .Produces<PrivateArtifactResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a specific artifact by its ID");

        privateGroup.MapPut("/{artifactId:int}", UpdateArtifact)
            .WithName(nameof(UpdateArtifact))
            .Accepts<UpdateArtifactRequest>("application/json")
            .Produces<PrivateArtifactResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Update an existing artifact");

        privateGroup.MapDelete("/{artifactId:int}", DeleteArtifact)
            .WithName(nameof(DeleteArtifact))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete an existing artifact");

        return route;
    }

    private static async Task<Results<Ok<List<PublicArtifactResponse>>, NotFound>> GetAllPublicArtifacts(ISender sender, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(new GetAllPublicArtifactsQuery(), ct));

    private static async Task<Results<Ok<List<PrivateArtifactResponse>>, NotFound>> GetAllPrivateArtifacts(ISender sender, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(new GetAllPrivateArtifactsQuery(), ct));

    private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> CreateArtifact(ISender sender, CreateArtifactRequest request, CancellationToken ct)
    {
        var artifact = await sender.Send(new CreateArtifactCommand(
            request.Name, request.CatalogNumber, request.PublicNarrative,
            request.DateDiscovered, request.Type, request.SiteId, request.Description), ct);
        return TypedResults.Ok(artifact);
    }

    private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> GetPrivateArtifactById(ISender sender, int artifactId, CancellationToken ct)
    {
        var artifact = await sender.Send(new GetPrivateArtifactByIdQuery(artifactId), ct);
        return artifact is null ? TypedResults.NotFound() : TypedResults.Ok(artifact);
    }

    private static async Task<Results<Ok<PublicArtifactResponse>, NotFound>> GetPublicArtifactById(ISender sender, int artifactId, CancellationToken ct)
    {
        var artifact = await sender.Send(new GetPublicArtifactByIdQuery(artifactId), ct);
        return artifact is null ? TypedResults.NotFound() : TypedResults.Ok(artifact);
    }

    private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> UpdateArtifact(ISender sender, int artifactId, UpdateArtifactRequest request, CancellationToken ct)
    {
        var artifact = await sender.Send(new UpdateArtifactCommand(
            artifactId, request.Name, request.CatalogNumber, request.PublicNarrative,
            request.DateDiscovered, request.Type, request.SiteId, request.Description), ct);
        return artifact is null ? TypedResults.NotFound() : TypedResults.Ok(artifact);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteArtifact(ISender sender, int artifactId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteArtifactCommand(artifactId), ct);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
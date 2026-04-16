using AleonAPI.Filters;
using AleonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AleonAPI.Endpoints.Artifact;

public static class ArtifactEndpoints
{
    //public group 

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
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithDescription("Endpoints that expose artifact data, accessible only to admin users");


        publicGroup.MapGet("/{artifactId:int}", GetPublicArtifactById)
       .WithName(nameof(GetPublicArtifactById))
       .WithDescription("Returns a specific artifact by its ID, including all private data")
       .Produces<PublicArtifactResponse>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status400BadRequest)
       .Produces(StatusCodes.Status404NotFound)
       .Produces(StatusCodes.Status500InternalServerError)
       .WithSummary("Get a specific artifact by its ID");


        //private group 

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
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Get all private artifacts");

        privateGroup.MapPost("", CreateArtifact)
        .WithName(nameof(CreateArtifact))
        .WithDescription("Creates a new artifact with the provided data, accessible only to admin users")
        .Accepts<CreateArtifactRequest>("application/json")
        .Produces<PrivateArtifactResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Create a new artifact");

        privateGroup.MapGet("/{artifactId:int}", GetPrivateArtifactById)
        .WithName(nameof(GetPrivateArtifactById))
        .WithDescription("Returns a specific artifact by its ID, including all private data")
        .Produces<PrivateArtifactResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Get a specific artifact by its ID");


        privateGroup.MapPut("/{artifactId:int}", UpdateArtifact)
        .WithName(nameof(UpdateArtifact))
        .WithDescription("Updates an existing artifact with the provided data, accessible only to admin users")
        .Accepts<UpdateArtifactRequest>("application/json")
        .Produces<PrivateArtifactResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Update an existing artifact");


        privateGroup.MapDelete("/{artifactId:int}", DeleteArtifact)
        .WithName(nameof(DeleteArtifact))
        .WithDescription("Deletes an existing artifact, accessible only to admin users")
        .Produces<bool>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithSummary("Delete an existing artifact");



        return route;
    }


    private static async Task<Results<Ok<List<PublicArtifactResponse>>, NotFound>> GetAllPublicArtifacts(IArtifactService artifactService, CancellationToken ct)
    {
        var artifacts = await artifactService.GetAllPublicArtifactsAsync(ct);
        if (artifacts is null) return TypedResults.NotFound();
        return TypedResults.Ok(artifacts);
    }

    private static async Task<Results<Ok<List<PrivateArtifactResponse>>, NotFound>> GetAllPrivateArtifacts(IArtifactService artifactService, CancellationToken ct)
    {
        var artifacts = await artifactService.GetAllPrivateArtifactsAsync(ct);
        if (artifacts is null) return TypedResults.NotFound();
        return TypedResults.Ok(artifacts);
    }

    private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> CreateArtifact(
        IArtifactService artifactService,
        CreateArtifactRequest request,
        CancellationToken ct)
    {
        var artifact = await artifactService.CreateArtifactAsync(request, ct);
        return TypedResults.Ok(artifact);
    }

    private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> GetPrivateArtifactById(
        IArtifactService artifactService,
        int artifactId,
        CancellationToken ct)
    {
        var artifact = await artifactService.GetPrivateArtifactByIdAsync(artifactId, ct);
        if (artifact is null) return TypedResults.NotFound();
        return TypedResults.Ok(artifact);
    }

    private static async Task<Results<Ok<PublicArtifactResponse>, NotFound>> GetPublicArtifactById(
        IArtifactService artifactService,
        int artifactId,
        CancellationToken ct)
    {
        var artifact = await artifactService.GetPublicArtifactByIdAsync(artifactId, ct);
        if (artifact is null) return TypedResults.NotFound();
        return TypedResults.Ok(artifact);
    }

    private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> UpdateArtifact(
        IArtifactService artifactService,
        int artifactId,
        UpdateArtifactRequest request,
        CancellationToken ct)
    {
        var artifact = await artifactService.UpdateArtifactAsync(artifactId, request, ct);
        if (artifact is null) return TypedResults.NotFound();
        return TypedResults.Ok(artifact);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteArtifact(
        IArtifactService artifactService,
        int artifactId,
        CancellationToken ct)
    {
        var result = await artifactService.DeleteArtifactAsync(artifactId, ct);
        if (!result) return TypedResults.NotFound();
        return TypedResults.NoContent();
    }


}
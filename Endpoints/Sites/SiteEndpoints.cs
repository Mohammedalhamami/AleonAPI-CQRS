using AleonAPI.Features.Sites.Commands.CreateSite;
using AleonAPI.Features.Sites.Commands.DeleteSite;
using AleonAPI.Features.Sites.Commands.UpdateSite;
using AleonAPI.Features.Sites.Queries.GetAllPrivateSites;
using AleonAPI.Features.Sites.Queries.GetAllPublicSites;
using AleonAPI.Features.Sites.Queries.GetPrivateSiteById;
using AleonAPI.Features.Sites.Queries.GetPublicSiteById;
using AleonAPI.Features.Artifacts.Queries.GetPublicArtifactsBySiteId;
using AleonAPI.Features.Artifacts.Queries.GetPrivateArtifactsBySiteId;
using AleonAPI.Filters;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AleonAPI.Endpoints.Sites;

public static class SiteEndpoints
{
    public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder route)
    {
        var publicGroup = route.MapGroup("api/public/sites")
            .AllowAnonymous()
            .WithSummary("Public site endpoints")
            .WithDescription("Endpoints that expose public sites data")
            .WithTags("Sites - Public")
            .AddEndpointFilter<ExceptionHandlingFilter>();

        publicGroup.MapGet("", GetAllPublicSites)
            .WithName(nameof(GetAllPublicSites))
            .WithDescription("Returns all sites with their public data")
            .Produces<IEnumerable<PublicSiteResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all public sites");

        publicGroup.MapGet("{siteId:int}", GetPublicSiteById)
            .WithName(nameof(GetPublicSiteById))
            .WithDescription("Returns a single site with its public data based on the provided site ID")
            .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a public site by ID");

        publicGroup.MapGet("/{siteId:int}/artifacts", GetPublicArtifactsBySiteId)
            .WithName(nameof(GetPublicArtifactsBySiteId))
            .Produces<List<PublicArtifactResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithDescription("Endpoints that expose artifact data based on site id");

        var privateGroup = route.MapGroup("api/private/sites")
            .RequireAuthorization()
            .WithSummary("Private site endpoints")
            .WithDescription("Endpoints that expose private sites data, accessible only to admin users")
            .WithTags("Sites - Private")
            .AddEndpointFilter<ExceptionHandlingFilter>();

        privateGroup.MapGet("", GetAllPrivateSites)
            .WithName(nameof(GetAllPrivateSites))
            .WithDescription("Returns all sites with their private data, accessible only to admin users")
            .Produces<IEnumerable<PrivateSiteResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all private sites");

        privateGroup.MapGet("{siteId:int}", GetPrivateSiteById)
            .WithName(nameof(GetPrivateSiteById))
            .WithDescription("Returns a single site with its Private data based on the provided site ID")
            .Produces<PrivateSiteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get a Private site by ID");

        privateGroup.MapPost("", CreateSite)
            .WithName(nameof(CreateSite))
            .WithDescription("Creates a new site with the provided data, accessible only to admin users")
            .Accepts<CreateSiteRequest>("application/json")
            .Produces<PrivateSiteResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new site");

        privateGroup.MapPut("/{siteId:int}", UpdateSite)
            .WithName(nameof(UpdateSite))
            .Accepts<UpdateSiteRequest>("application/json")
            .Produces<PrivateSiteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .WithSummary("Update a Site")
            .WithDescription("Updates an existing site and returns its updated details.");

        privateGroup.MapDelete("/{siteId:int}", DeleteSite)
            .WithName(nameof(DeleteSite))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Delete a Site")
            .WithDescription("Deletes an existing site based on the provided site ID.");

        privateGroup.MapGet("/{siteId:int}/artifacts", GetPrivateArtifactsBySiteId)
            .WithName(nameof(GetPrivateArtifactsBySiteId))
            .Produces<List<PrivateArtifactResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithDescription("Endpoints that expose artifact data based on site id");

        return route;
    }

    private static async Task<Ok<IEnumerable<PublicSiteResponse>>> GetAllPublicSites(ISender sender, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(new GetAllPublicSitesQuery(), ct));

    private static async Task<Results<Ok<PublicSiteResponse>, NotFound>> GetPublicSiteById(int siteId, ISender sender, CancellationToken ct)
    {
        var site = await sender.Send(new GetPublicSiteByIdQuery(siteId), ct);
        return site is null ? TypedResults.NotFound() : TypedResults.Ok(site);
    }

    private static async Task<Ok<IEnumerable<PrivateSiteResponse>>> GetAllPrivateSites(ISender sender, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(new GetAllPrivateSitesQuery(), ct));

    private static async Task<Results<Ok<PrivateSiteResponse>, NotFound>> GetPrivateSiteById(int siteId, ISender sender, CancellationToken ct)
    {
        var site = await sender.Send(new GetPrivateSiteByIdQuery(siteId), ct);
        return site is null ? TypedResults.NotFound() : TypedResults.Ok(site);
    }

    private static async Task<Results<Created<PrivateSiteResponse>, BadRequest>> CreateSite(CreateSiteRequest request, ISender sender, CancellationToken ct)
    {
        var created = await sender.Send(new CreateSiteCommand(
            request.Name, request.Location, request.Latitude, request.Longitude,
            request.Description, request.PublicNarrative, request.AeonNarrative), ct);
        return TypedResults.Created($"/api/private/sites/{created.Id}", created);
    }

    private static async Task<Results<Ok<PrivateSiteResponse>, NotFound, ValidationProblem>> UpdateSite(int siteId, UpdateSiteRequest request, ISender sender, CancellationToken ct)
    {
        var updated = await sender.Send(new UpdateSiteCommand(
            siteId, request.Name, request.Location, request.Latitude, request.Longitude,
            request.Description, request.PublicNarrative, request.AeonNarrative), ct);
        return updated is null ? TypedResults.NotFound() : TypedResults.Ok(updated);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteSite(int siteId, ISender sender, CancellationToken ct)
    {
        var deleted = await sender.Send(new DeleteSiteCommand(siteId), ct);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<List<PublicArtifactResponse>>, NotFound>> GetPublicArtifactsBySiteId(ISender sender, int siteId, CancellationToken ct)
    {
        var artifacts = await sender.Send(new GetPublicArtifactsBySiteIdQuery(siteId), ct);
        return artifacts is null ? TypedResults.NotFound() : TypedResults.Ok(artifacts);
    }

    private static async Task<Results<Ok<List<PrivateArtifactResponse>>, NotFound>> GetPrivateArtifactsBySiteId(ISender sender, int siteId, CancellationToken ct)
    {
        var artifacts = await sender.Send(new GetPrivateArtifactsBySiteIdQuery(siteId), ct);
        return artifacts is null ? TypedResults.NotFound() : TypedResults.Ok(artifacts);
    }
}

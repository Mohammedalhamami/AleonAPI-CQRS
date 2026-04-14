using System;
using System.Text.RegularExpressions;
using AleonAPI.Filters;
using AleonAPI.Models.Request;
using AleonAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;

namespace AleonAPI.Endpoints.Sites;

public static class SiteEndpoints
{

    //endpoints groups for Sites

    public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder route)
    {

        //first group
        var publicGroup = route.MapGroup("api/public/sites")
        .AllowAnonymous()
        .WithSummary("Public site endpoints")
        .WithDescription("Endpoints that expose public sites data")
        .WithTags("Sites - Public")
        .AddEndpointFilter<ExceptionHandlingFilter>();

        //then endpoints 
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


        //private group 

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
        .Produces(StatusCodes.Status404NotFound)
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
        .WithSummary("Create a new site")
        .WithDescription("Creates a new site with the provided data, accessible only to admin users");

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

        return route;

    }
    //method handlers for sites.

    private static async Task<Ok<IEnumerable<PublicSiteResponse>>> GetAllPublicSites(ISiteService siteService, CancellationToken ct)
    {
        var sites = await siteService.GetAllPublicSitesAsync(ct);
        return TypedResults.Ok(sites);
    }


    private static async Task<Results<Ok<PublicSiteResponse>, NotFound>> GetPublicSiteById(int siteId, ISiteService siteService, CancellationToken ct)
    {
        var site = await siteService.GetPublicSiteByIdAsync(siteId, ct);

        if (site == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(site);
    }
    private static async Task<Ok<IEnumerable<PrivateSiteResponse>>> GetAllPrivateSites(ISiteService siteService, CancellationToken ct)
    {
        var sites = await siteService.GetAllPrivateSitesAsync(ct);
        return TypedResults.Ok(sites);
    }

    private static async Task<Results<Ok<PrivateSiteResponse>, NotFound>> GetPrivateSiteById(int siteId, ISiteService siteService, CancellationToken ct)
    {
        var site = await siteService.GetPrivateSiteByIdAsync(siteId, ct);

        if (site == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(site);
    }

    private static async Task<Results<Created<PrivateSiteResponse>, BadRequest>> CreateSite(CreateSiteRequest request, ISiteService siteService, CancellationToken ct)
    {
        var createdSite = await siteService.CreateSiteAsync(request, ct);
        return TypedResults.Created($"/api/private/sites/{createdSite.Id}", createdSite);
    }

    private static async Task<Results<Ok<PrivateSiteResponse>, NotFound, ValidationProblem>> UpdateSite(int siteId, UpdateSiteRequest request, ISiteService siteService, CancellationToken ct)
    {

        var updatedSite = await siteService.UpdateSiteAsync(siteId, request, ct);
        return updatedSite != null ? TypedResults.Ok(updatedSite) : TypedResults.NotFound();


    }

    private static async Task<Results<NoContent, NotFound>> DeleteSite(int siteId, ISiteService siteService, CancellationToken ct)
    {
        var deleted = await siteService.DeleteSiteAsync(siteId, ct);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }

}

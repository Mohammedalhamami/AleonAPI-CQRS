using System;
using System.Text.RegularExpressions;
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
      .WithTags("Sites - Public");
        
//then endpoints 
        publicGroup.MapGet("", GetAllPublicSites)
        .WithName(nameof(GetAllPublicSites))
        .WithDescription("Returns all sites with their public data")
        .Produces<IEnumerable<PublicSiteResponse>>(StatusCodes.Status200OK)
        .WithSummary("Get all public sites");

        return route;

   }
    //method handlers for sites.

    private static async Task<Ok<IEnumerable<PublicSiteResponse>>> GetAllPublicSites(ISiteService siteService, CancellationToken ct)
    {
        var sites = await siteService.GetAllPublicSitesAsync(ct);
        return  TypedResults.Ok(sites);
    }

}

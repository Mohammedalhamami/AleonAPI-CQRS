using AleonAPI.Features.CatalogRecords.Commands.CreateCatalogRecord;
using AleonAPI.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace AleonAPI.Endpoints.CatalogRecrod;

public static class CatalogRecrodEndpoints
{
    public static void MapCatalogRecordEndpoints(this IEndpointRouteBuilder app)
    {
        //group.
        var group = app.MapGroup("api/private/catalog-record")
            .RequireAuthorization()
            .WithTags("CatalogRecords")
            .WithSummary("Catalog record endpoints")
            .WithDescription("Endpoints that expose catalog record data")
            .AddEndpointFilter<ExceptionHandlingFilter>();

        //ednpoints
        group.MapPost("", CreateCatalogRecord)
            .WithName("CreateCatalogRecord")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<Results<Created<int>, BadRequest<string>>> CreateCatalogRecord(
       ISender sender,
       ClaimsPrincipal user,  // Automatically injected from the validated JWT token
       CreateCatalogRecordRequest request,
       CancellationToken ct)
    {
        // 1. Get the authenticated user's ID from the JWT
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // 2. Map the incoming JSON to the MediatR Command
        var command = new CreateCatalogRecordCommand(
            request.ArtifactId,
            request.CatalogNumber,
            request.DateAdded,
            userId,  // Auto-filled from JWT
            userId,  // Auto-filled from JWT
            request.CatalogStatus,
            request.DateSubmitted
        );

        // 3. Send the mapped command!
        var resultId = await sender.Send(command, ct);
        
        if (resultId > 0)
        {
            // Returns 201 Created along with the ID!
            return TypedResults.Created($"/api/private/catalog-record/{resultId}", resultId);
        }
        
        return TypedResults.BadRequest("Failed to create record. Verify that the ArtifactId exists and the User is valid.");
    }


}
using AleonAPI.Features.Countries.Commands.CreateCountry;
using AleonAPI.Features.Countries.Commands.ImportCountries;
using AleonAPI.Features.Countries.Queries.GetAllCountries;
using AleonAPI.Filters;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AleonAPI.Endpoints.Country;

public static class CountryEndpoints
{
    public static IEndpointRouteBuilder MapCountryEndpoints(this IEndpointRouteBuilder route)
    {
        var group = route.MapGroup("api/public/countries")
            .AllowAnonymous()
            .WithSummary("get all countries")
            .WithDescription("Endpoints that expose countries data")
            .WithTags("Countries")
            .AddEndpointFilter<ExceptionHandlingFilter>();

        group.MapGet("/", GetAllCountries)
            .WithName(nameof(GetAllCountries))
            .Produces<IEnumerable<CountryResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all countries");

        group.MapPost("/", CreateCountry)
            .WithName(nameof(CreateCountry))
            .Produces<CountryResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Create a new country");

        group.MapPost("/import", ImportCountries)
            .DisableAntiforgery()
            .WithSummary("Import countries from Excel");

        return route;
    }

    private static async Task<Ok<IEnumerable<CountryResponse>>> GetAllCountries(ISender sender, [AsParameters] GetAllCountriesQuery query, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(query, ct));


    private static async Task<Ok<CountryResponse>> CreateCountry(ISender sender, CreateCountryCommand command, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(command, ct));

    private static async Task<Accepted> ImportCountries(ISender sender, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file was uploaded.");

        // 1. Generate a temporary file path on the operating system
        var tempFilePath = Path.GetTempFileName();

        // 2. Stream the uploaded data directly into the hard drive file
        await using var stream = new FileStream(tempFilePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        stream.Close(); // Unlock the file so the Background thread can read it!

        // 3. Immediately send the string Path to MediatR so it can Queue it!
        await sender.Send(new ImportCountriesCommand(tempFilePath), ct);

        // 4. Return extremely fast HTTP 202 to user. Beautiful UX!
        return TypedResults.Accepted(string.Empty);
    }
}

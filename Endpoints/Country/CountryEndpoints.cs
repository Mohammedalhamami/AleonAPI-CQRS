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

    private static async Task<Ok<IEnumerable<CountryResponse>>> GetAllCountries(ISender sender, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(new GetAllCountriesQuery(), ct));


    private static async Task<Ok<CountryResponse>> CreateCountry(ISender sender, CreateCountryCommand command, CancellationToken ct)
        => TypedResults.Ok(await sender.Send(command, ct));

    private static async Task<Ok<string>> ImportCountries(ISender sender, IFormFile file, CancellationToken ct)
    {

        if (file == null || file.Length == 0)
            throw new ArgumentException("No file was uploaded.");

        await using var stream = file.OpenReadStream();

        return TypedResults.Ok($"{await sender.Send(new ImportCountriesCommand(stream), ct)} countries imported successfully.");
    }
}

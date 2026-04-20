namespace AleonAPI.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery : IRequest<IEnumerable<CountryResponse>>;


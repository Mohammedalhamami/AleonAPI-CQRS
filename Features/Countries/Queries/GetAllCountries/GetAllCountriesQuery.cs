namespace AleonAPI.Features.Countries.Queries.GetAllCountries;

public record GetAllCountriesQuery(int PageNumber = 1, int PageSize = 50) : IRequest<IEnumerable<CountryResponse>>;

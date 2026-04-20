namespace AleonAPI.Features.Countries.Commands.ImportCountries;

public record ImportCountriesCommand(Stream FileStream) : IRequest<int>;

namespace AleonAPI.Features.Countries.Commands.ImportCountries;

public record ImportCountriesCommand(string FilePath) : IRequest;

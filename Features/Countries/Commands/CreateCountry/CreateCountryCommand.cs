using AleonAPI.Enums;

namespace AleonAPI.Features.Countries.Commands.CreateCountry;

public class CreateCountryCommand : IRequest<CountryResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Continent Contenients { get; set; } = default;
}

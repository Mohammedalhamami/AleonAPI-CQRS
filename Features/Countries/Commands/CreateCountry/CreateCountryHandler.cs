using AleonAPI.Enums;

namespace AleonAPI.Features.Countries.Commands.CreateCountry;

public class CreateCountryHandler(ApplicationDbContext context) : IRequestHandler<CreateCountryCommand, CountryResponse>
{
    public async Task<CountryResponse> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Contenients))
        {
            throw new ArgumentException("Invalid Continent value provided.");
        }

        var country = new Country
        {
            Name = request.Name,
            Code = request.Code,
            Contenients = request.Contenients
        };

        await context.Countries.AddAsync(country, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new CountryResponse
        {
            Id = country.Id,
            Name = country.Name,
            Code = country.Code,
            Continents = country.Contenients.ToString()
        };
    }
}

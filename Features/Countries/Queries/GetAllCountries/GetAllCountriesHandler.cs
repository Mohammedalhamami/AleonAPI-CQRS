using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Features.Countries.Queries.GetAllCountries;

public class GetAllCountriesHandler(ApplicationDbContext context) : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryResponse>>
{
    public async Task<IEnumerable<CountryResponse>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        return await context.Countries
        .AsNoTracking()
        .Select(c => new CountryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            Continents = c.Contenients.ToString()
        }).ToListAsync(cancellationToken);
    }
}

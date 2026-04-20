using ClosedXML.Excel;
using AleonAPI.Enums;

namespace AleonAPI.Features.Countries.Commands.ImportCountries;
// Features/Countries/Commands/ImportCountries/ImportCountriesHandler.cs

public class ImportCountriesHandler(ApplicationDbContext context) : IRequestHandler<ImportCountriesCommand, int>
{
    public async Task<int> Handle(ImportCountriesCommand request, CancellationToken ct)
    {
        var countriesToInsert = new List<Country>();

        // 1. Open the Excel workbook from the stream
        using var workbook = new XLWorkbook(request.FileStream);
        var worksheet = workbook.Worksheet(1); // Get the first tab
        var rows = worksheet.RowsUsed().Skip(1); // Skip the header row!

        // 2. Loop through the rows and map to your Model
        foreach (var row in rows)
        {
            var country = new Country
            {
                Name = row.Cell(1).GetString(),         // Column A
                Code = row.Cell(2).GetString(),         // Column B
                Contenients = Enum.Parse<Continent>(row.Cell(3).GetString()) // Column C
            };

            countriesToInsert.Add(country);
        }

        // 3. Bulk save to the database!
        await context.Countries.AddRangeAsync(countriesToInsert, ct);
        await context.SaveChangesAsync(ct);

        return countriesToInsert.Count;
    }
}

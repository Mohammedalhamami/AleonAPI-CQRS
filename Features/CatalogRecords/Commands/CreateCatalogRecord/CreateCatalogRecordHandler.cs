using Microsoft.EntityFrameworkCore;
using AleonAPI.Enums;

namespace AleonAPI.Features.CatalogRecords.Commands.CreateCatalogRecord;

public class CreateCatalogRecordHandler(ApplicationDbContext context) : IRequestHandler<CreateCatalogRecordCommand, int>
{
    public async Task<int> Handle(CreateCatalogRecordCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<CatalogStatus>(request.CatalogStatus, true, out var catalogStatus))
            throw new ArgumentException($"Invalid catalog status '{request.CatalogStatus}'. " +
                $"Allowed values are: {string.Join(", ", Enum.GetNames(typeof(CatalogStatus)))}");


        var artifact = await context.Artifacts.AnyAsync(x => x.Id == request.ArtifactId, ct);
        if (!artifact)
        {
            return 0;
        }

        var verifier = await context.Users.AnyAsync(x => x.Id == request.VerifiedById, ct);
        if (!verifier)
        {
            return 0;
        }

        var submitter = await context.Users.AnyAsync(x => x.Id == request.SubmittedById, ct);
        if (!submitter)
        {
            return 0;
        }

        var catalogRecord = new CatalogRecord
        {
            ArtifactId = request.ArtifactId,
            CatalogNumber = request.CatalogNumber,
            DateAdded = request.DateAdded,
            SubmittedById = request.SubmittedById,
            VerifiedById = request.VerifiedById,
            CatalogStatus = catalogStatus.ToString(), // Ensures exact casing match with enum
            DateSubmitted = request.DateSubmitted
        };

        context.CatalogRecords.Add(catalogRecord);
        await context.SaveChangesAsync(ct);

        // This guarantees Entity Framework saved it and generated an ID.
        return catalogRecord.Id;
    }
}

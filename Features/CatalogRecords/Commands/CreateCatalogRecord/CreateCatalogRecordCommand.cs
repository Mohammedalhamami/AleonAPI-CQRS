
namespace AleonAPI.Features.CatalogRecords.Commands.CreateCatalogRecord;

public record CreateCatalogRecordCommand(
    int ArtifactId,
    string CatalogNumber,
    DateTime DateAdded,
    string SubmittedById,
    string VerifiedById,
    string CatalogStatus,
    DateTime DateSubmitted
) : IRequest<int>;

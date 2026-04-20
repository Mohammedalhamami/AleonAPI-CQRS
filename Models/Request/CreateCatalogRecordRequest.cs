namespace AleonAPI.Models.Request;

public class CreateCatalogRecordRequest
{
    [Required]
    public int ArtifactId { get; set; }

    [Required]
    public string CatalogNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    public string CatalogStatus { get; set; } = Enums.CatalogStatus.Draft.ToString(); // Default to Draft, can be "Verified" or "Archived"

    public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
}

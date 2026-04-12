using Microsoft.AspNetCore.Identity;

namespace AleonAPI.Models;

public class User : IdentityUser
{
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<CatalogRecord> SubmittedCatalogRecords { get; set; } = [];
    public ICollection<CatalogRecord> VerifiedCatalogRecords { get; set; } = [];
    public ICollection<ArtifactMediaFile> UploadedMedia { get; set; } = [];
   
}
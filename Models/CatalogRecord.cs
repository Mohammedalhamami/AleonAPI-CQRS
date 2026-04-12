using System;
using System.Collections.ObjectModel;

namespace AleonAPI.Models;

public class CatalogRecord
{

    public int Id { get; set; }
    public int ArtifactId { get; set; }
    // Navigation property
    public Artifact? Artifact { get; set; } = null!;
    public string CatalogNumber { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [Required]
    public string SubmittedById { get; set; } = string.Empty; //fk for User model.
    public User? SubmittedBy { get; set; } = null!; // Navigation property to User

    [Required]
    public string VerifiedById { get; set; } = string.Empty; //fk for User model.
    public User? VerifiedBy { get; set; } = null!; // Navigation property

    public string CatalogStatus { get; set; } = Enums.CatalogStatus.Draft.ToString(); // Default to Draft, can be "Verified" or "Archived"

    public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

    public ICollection<CatalogNote> Notes { get; set; } = [];

}



namespace AleonAPI.Models;

public class CatalogNote
{
    public int Id { get; set; }
    [Required]
    public int CatalogRecordId { get; set; } // Foreign key to CatalogRecord
    public CatalogRecord? CatalogRecord { get; set; } = null!; // Navigation property
    public string AuthorId { get; set; } = string.Empty; //fk for User model.
    public User? Author { get; set; } = null!; // Navigation property to User

    [Required, MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}

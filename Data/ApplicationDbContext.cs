using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AleonAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{

    public override DbSet<User> Users { get; set; }
    public DbSet<Artifact> Artifacts { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<CatalogRecord> CatalogRecords { get; set; }
    public DbSet<CatalogNote> CatalogNotes { get; set; }
    public DbSet<ArtifactMediaFile> ArtifactMediaFiles { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints
        modelBuilder.Entity<CatalogRecord>()
            .HasOne(cr => cr.SubmittedBy)
            .WithMany(u => u.SubmittedCatalogRecords)
            .HasForeignKey(cr => cr.SubmittedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CatalogRecord>()
            .HasOne(cr => cr.VerifiedBy)
            .WithMany(u => u.VerifiedCatalogRecords)
            .HasForeignKey(cr => cr.VerifiedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ArtifactMediaFile>()
            .HasOne(amf => amf.Artifact)
            .WithMany(a => a.MediaFiles)
            .HasForeignKey(amf => amf.ArtifactId);
    }

}
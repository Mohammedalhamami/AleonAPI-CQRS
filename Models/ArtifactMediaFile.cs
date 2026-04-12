using System;

namespace AleonAPI.Models;

public class ArtifactMediaFile
{
    public int Id { get; set; }
    public int ArtifactId { get; set; }
    // Navigation property
    public Artifact? Artifact { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "image/jpeg";
    public byte[] Data { get; set; } = Array.Empty<byte>();
    //option to mark one media file as primary for the artifact
    public bool IsPrimary { get; set; } = false;

}

using System;

namespace AleonAPI.Models.Request;

public class CreateSiteRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Coordinates { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? PublicNarrative { get; set; }

    [MaxLength(2000)]
    public string? AeonNarrative { get; set; }
}
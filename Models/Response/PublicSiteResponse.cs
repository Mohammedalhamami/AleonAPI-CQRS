using System;

namespace AleonAPI.Models.Response;

public class PublicSiteResponse
{

    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Description { get; set; }
    public string? PublicNarrative { get; set; }

}

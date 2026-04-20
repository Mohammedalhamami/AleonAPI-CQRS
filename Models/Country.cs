using AleonAPI.Enums;

namespace AleonAPI.Models;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Continent Contenients { get; set; } = default;
}

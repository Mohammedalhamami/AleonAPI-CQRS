using AleonAPI.Enums;

namespace AleonAPI.Models.Request;

public class CreateCountryRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Code { get; set; } = string.Empty;
    [Required]
    public Continent Contenients { get; set; } = default;
}

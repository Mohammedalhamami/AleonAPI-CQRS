namespace AleonAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Continent : byte
{
    Africa,
    Antarctica,
    Asia,
    Europe,
    NorthAmerica,
    Oceania,
    SouthAmerica
}

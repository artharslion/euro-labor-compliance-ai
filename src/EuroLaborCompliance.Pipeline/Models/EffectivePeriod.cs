namespace EuroLaborCompliance.Pipeline.Models;

public class EffectivePeriod
{
    [JsonPropertyName("validFrom")]
    public required string ValidFrom { get; init; }

    [JsonPropertyName("validTo")]
    public string? ValidTo { get; init; }
}

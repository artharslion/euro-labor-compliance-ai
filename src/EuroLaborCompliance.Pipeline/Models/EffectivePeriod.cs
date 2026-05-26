namespace EuroLaborCompliance.Pipeline.Models;

public class EffectivePeriod
{
    [JsonPropertyName("validFrom")]
    public string ValidFrom { get; set; }

    [JsonPropertyName("validTo")]
    public string? ValidTo { get; set; }
}

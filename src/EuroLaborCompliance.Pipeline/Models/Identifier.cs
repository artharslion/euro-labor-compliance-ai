namespace EuroLaborCompliance.Pipeline.Models;

public class Identifier
{
    [JsonPropertyName("value")]
    public required string Value { get; init; }

    [JsonPropertyName("schemeAgencyId")]
    public required string SchemeAgencyId { get; init; }
}

public class IdValue
{
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

public class Id
{
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

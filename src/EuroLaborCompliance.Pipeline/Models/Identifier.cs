namespace EuroLaborCompliance.Pipeline.Models;

public class Identifier
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("schemeAgencyId")]
    public string SchemeAgencyId { get; set; }
}

public class IdValue
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class Id
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

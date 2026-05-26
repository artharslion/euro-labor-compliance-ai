namespace EuroLaborCompliance.Pipeline.Models;

public class LabourAgreements
{
    [JsonPropertyName("industryIdentifier")]
    public Identifier[]? IndustryIdentifier { get; init; }

    [JsonPropertyName("collectiveLabourAgreement")]
    public LabourAgreement? CollectiveLabourAgreement { get; init; }

    [JsonPropertyName("customLabourAgreement")]
    public LabourAgreement? CustomLabourAgreement { get; init; }
}

public class LabourAgreement
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("typeCode")]
    public required LabourAgreementType TypeCode { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }
}

public class PositionProfile
{
    [JsonPropertyName("positionId")]
    public string? PositionId { get; init; }

    [JsonPropertyName("positionTitle")]
    public string? PositionTitle { get; init; }

    [JsonPropertyName("origin")]
    public string? Origin { get; init; }

    [JsonPropertyName("referenceTitle")]
    public string? ReferenceTitle { get; init; }

    [JsonPropertyName("workDescription")]
    public string? WorkDescription { get; init; }
}

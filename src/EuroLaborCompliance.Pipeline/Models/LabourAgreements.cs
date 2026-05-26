namespace EuroLaborCompliance.Pipeline.Models;

public class LabourAgreements
{
    [JsonPropertyName("industryIdentifier")]
    public Identifier[]? IndustryIdentifier { get; set; }

    [JsonPropertyName("collectiveLabourAgreement")]
    public LabourAgreement? CollectiveLabourAgreement { get; set; }

    [JsonPropertyName("customLabourAgreement")]
    public LabourAgreement? CustomLabourAgreement { get; set; }
}

public class LabourAgreement
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("typeCode")]
    public LabourAgreementType TypeCode { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }
}

public class PositionProfile
{
    [JsonPropertyName("positionId")]
    public string? PositionId { get; set; }

    [JsonPropertyName("positionTitle")]
    public string? PositionTitle { get; set; }

    [JsonPropertyName("origin")]
    public string? Origin { get; set; }

    [JsonPropertyName("referenceTitle")]
    public string? ReferenceTitle { get; set; }

    [JsonPropertyName("workDescription")]
    public string? WorkDescription { get; set; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class ArrangementLine
{
    [JsonPropertyName("lineId")]
    public string LineId { get; set; }

    [JsonPropertyName("amount")]
    public Amount Amount { get; set; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; set; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; set; }

    [JsonPropertyName("contributionSource")]
    public ContributionSource? ContributionSource { get; set; }

    [JsonPropertyName("ikbReference")]
    public IkbReference[]? IkbReference { get; set; }
}

public class ContributionSource
{
    [JsonPropertyName("type")]
    public ContributionSourceType Type { get; set; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; set; }

    [JsonPropertyName("employerAmount")]
    public Amount? EmployerAmount { get; set; }

    [JsonPropertyName("employeeAmount")]
    public Amount? EmployeeAmount { get; set; }
}

public class IkbReference
{
    [JsonPropertyName("arrangementId")]
    public string ArrangementId { get; set; }

    [JsonPropertyName("relation")]
    public IkbRelationType Relation { get; set; }
}

public class AllowanceArrangement
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("origin")]
    public string? Origin { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }

    [JsonPropertyName("typeCode")]
    public AllowanceCode TypeCode { get; set; }

    [JsonPropertyName("period")]
    public Period[]? Period { get; set; }

    [JsonPropertyName("line")]
    public ArrangementLine[]? Line { get; set; }

    [JsonPropertyName("payDate")]
    public string? PayDate { get; set; }

    [JsonPropertyName("reference")]
    public Id[]? Reference { get; set; }

    [JsonPropertyName("phaseOutScheme")]
    public PhaseOutScheme? PhaseOutScheme { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class PhaseOutScheme
{
    [JsonPropertyName("phaseOutDate")]
    public string? PhaseOutDate { get; set; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; set; }
}

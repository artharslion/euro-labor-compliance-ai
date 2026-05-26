namespace EuroLaborCompliance.Pipeline.Models;

public class ArrangementLine
{
    [JsonPropertyName("lineId")]
    public required string LineId { get; init; }

    [JsonPropertyName("amount")]
    public required Amount Amount { get; init; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; init; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; init; }

    [JsonPropertyName("contributionSource")]
    public ContributionSource? ContributionSource { get; init; }

    [JsonPropertyName("ikbReference")]
    public IkbReference[]? IkbReference { get; init; }
}

public class ContributionSource
{
    [JsonPropertyName("type")]
    public required ContributionSourceType Type { get; init; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; init; }

    [JsonPropertyName("employerAmount")]
    public Amount? EmployerAmount { get; init; }

    [JsonPropertyName("employeeAmount")]
    public Amount? EmployeeAmount { get; init; }
}

public class IkbReference
{
    [JsonPropertyName("arrangementId")]
    public required string ArrangementId { get; init; }

    [JsonPropertyName("relation")]
    public required IkbRelationType Relation { get; init; }
}

public class AllowanceArrangement
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("origin")]
    public string? Origin { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }

    [JsonPropertyName("typeCode")]
    public required AllowanceCode TypeCode { get; init; }

    [JsonPropertyName("period")]
    public Period[]? Period { get; init; }

    [JsonPropertyName("line")]
    public ArrangementLine[]? Line { get; init; }

    [JsonPropertyName("payDate")]
    public string? PayDate { get; init; }

    [JsonPropertyName("reference")]
    public Id[]? Reference { get; init; }

    [JsonPropertyName("phaseOutScheme")]
    public PhaseOutScheme? PhaseOutScheme { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

public class PhaseOutScheme
{
    [JsonPropertyName("phaseOutDate")]
    public string? PhaseOutDate { get; init; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; init; }
}

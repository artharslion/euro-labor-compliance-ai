namespace EuroLaborCompliance.Pipeline.Models;

public class RemunerationPackage
{
    [JsonPropertyName("origin")]
    public string? Origin { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }

    [JsonPropertyName("workDuration")]
    public WorkDuration? WorkDuration { get; init; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; init; }

    [JsonPropertyName("hourlyWageConversion")]
    public Amount? HourlyWageConversion { get; init; }

    [JsonPropertyName("salaryScale")]
    public SalaryScale[]? SalaryScale { get; init; }

    [JsonPropertyName("individualSalaryIncrease")]
    public IndividualSalaryIncrease[]? IndividualSalaryIncrease { get; init; }

    [JsonPropertyName("generalSalaryIncrease")]
    public GeneralSalaryIncrease[]? GeneralSalaryIncrease { get; init; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; init; }
}

public class SalaryScale
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; init; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("salaryStep")]
    public SalaryScaleStep[]? SalaryStep { get; init; }

    [JsonPropertyName("careerLevel")]
    public int? CareerLevel { get; init; }

    [JsonPropertyName("positionProfileReference")]
    public PositionProfileReference[]? PositionProfileReference { get; init; }
}

public class SalaryScaleStep
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("value")]
    public required double Value { get; init; }

    [JsonPropertyName("minimumWage")]
    public double? MinimumWage { get; init; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; init; }
}

public class PositionProfileReference
{
    [JsonPropertyName("positionId")]
    public required string PositionId { get; init; }
}

public class IndividualSalaryIncrease
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; init; }
}

public class GeneralSalaryIncrease
{
    [JsonPropertyName("percentage")]
    public double? Percentage { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }
}

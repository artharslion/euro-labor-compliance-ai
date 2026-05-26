namespace EuroLaborCompliance.Pipeline.Models;

public class RemunerationPackage
{
    [JsonPropertyName("origin")]
    public string? Origin { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }

    [JsonPropertyName("workDuration")]
    public WorkDuration? WorkDuration { get; set; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; set; }

    [JsonPropertyName("hourlyWageConversion")]
    public Amount? HourlyWageConversion { get; set; }

    [JsonPropertyName("salaryScale")]
    public SalaryScale[]? SalaryScale { get; set; }

    [JsonPropertyName("individualSalaryIncrease")]
    public IndividualSalaryIncrease[]? IndividualSalaryIncrease { get; set; }

    [JsonPropertyName("generalSalaryIncrease")]
    public GeneralSalaryIncrease[]? GeneralSalaryIncrease { get; set; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; set; }
}

public class SalaryScale
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("salaryStep")]
    public SalaryScaleStep[]? SalaryStep { get; set; }

    [JsonPropertyName("careerLevel")]
    public int? CareerLevel { get; set; }

    [JsonPropertyName("positionProfileReference")]
    public PositionProfileReference[]? PositionProfileReference { get; set; }
}

public class SalaryScaleStep
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("minimumWage")]
    public double? MinimumWage { get; set; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; set; }
}

public class PositionProfileReference
{
    [JsonPropertyName("positionId")]
    public string PositionId { get; set; }
}

public class IndividualSalaryIncrease
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("percentage")]
    public double? Percentage { get; set; }
}

public class GeneralSalaryIncrease
{
    [JsonPropertyName("percentage")]
    public double? Percentage { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }
}

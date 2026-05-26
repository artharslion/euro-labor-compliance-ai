namespace EuroLaborCompliance.Pipeline.Models;

public class Condition
{
    [JsonPropertyName("conditionType")]
    public required string ConditionType { get; init; }

    [JsonPropertyName("operator")]
    public string? Operator { get; init; }

    [JsonPropertyName("age")]
    public AgeCondition? Age { get; init; }

    [JsonPropertyName("duration")]
    public DurationCondition? Duration { get; init; }

    [JsonPropertyName("referenceDateType")]
    public string? ReferenceDateType { get; init; }

    [JsonPropertyName("occurrence")]
    public Occurrence? Occurrence { get; init; }

    [JsonPropertyName("positionProfileIds")]
    public string[]? PositionProfileIds { get; init; }

    [JsonPropertyName("salaryScale")]
    public SalaryScaleCondition? SalaryScale { get; init; }

    [JsonPropertyName("step")]
    public StepCondition? Step { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; init; }

    [JsonPropertyName("not")]
    public Condition? Not { get; init; }
}

public class AgeCondition
{
    [JsonPropertyName("minimumAge")]
    public double? MinimumAge { get; init; }

    [JsonPropertyName("maximumAge")]
    public double? MaximumAge { get; init; }
}

public class DurationCondition
{
    [JsonPropertyName("value")]
    public double? Value { get; init; }

    [JsonPropertyName("unitCode")]
    public IntervalCode? UnitCode { get; init; }
}

public class SalaryScaleCondition
{
    [JsonPropertyName("salaryScaleName")]
    public string? SalaryScaleName { get; init; }

    [JsonPropertyName("stepName")]
    public string? StepName { get; init; }
}

public class StepCondition
{
    [JsonPropertyName("stepValue")]
    public required double StepValue { get; init; }

    [JsonPropertyName("positionProfileReference")]
    public PositionProfileReference[]? PositionProfileReference { get; init; }
}

public class Occurrence
{
    [JsonPropertyName("occurrenceType")]
    public string? OccurrenceType { get; init; }

    [JsonPropertyName("date")]
    public string? Date { get; init; }

    [JsonPropertyName("recurringInterval")]
    public Interval? RecurringInterval { get; init; }

    [JsonPropertyName("event")]
    public string? Event { get; init; }

    [JsonPropertyName("offset")]
    public int? Offset { get; init; }

    [JsonPropertyName("eventName")]
    public string? EventName { get; init; }
}

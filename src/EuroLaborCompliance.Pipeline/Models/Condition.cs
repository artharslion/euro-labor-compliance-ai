namespace EuroLaborCompliance.Pipeline.Models;

public class Condition
{
    [JsonPropertyName("conditionType")]
    public string ConditionType { get; set; }

    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    [JsonPropertyName("age")]
    public AgeCondition? Age { get; set; }

    [JsonPropertyName("duration")]
    public DurationCondition? Duration { get; set; }

    [JsonPropertyName("referenceDateType")]
    public string? ReferenceDateType { get; set; }

    [JsonPropertyName("occurrence")]
    public Occurrence? Occurrence { get; set; }

    [JsonPropertyName("positionProfileIds")]
    public string[]? PositionProfileIds { get; set; }

    [JsonPropertyName("salaryScale")]
    public SalaryScaleCondition? SalaryScale { get; set; }

    [JsonPropertyName("step")]
    public StepCondition? Step { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; set; }

    [JsonPropertyName("not")]
    public Condition? Not { get; set; }
}

public class AgeCondition
{
    [JsonPropertyName("minimumAge")]
    public double? MinimumAge { get; set; }

    [JsonPropertyName("maximumAge")]
    public double? MaximumAge { get; set; }
}

public class DurationCondition
{
    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("unitCode")]
    public IntervalCode? UnitCode { get; set; }
}

public class SalaryScaleCondition
{
    [JsonPropertyName("salaryScaleName")]
    public string? SalaryScaleName { get; set; }

    [JsonPropertyName("stepName")]
    public string? StepName { get; set; }
}

public class StepCondition
{
    [JsonPropertyName("stepValue")]
    public double StepValue { get; set; }

    [JsonPropertyName("positionProfileReference")]
    public PositionProfileReference[]? PositionProfileReference { get; set; }
}

public class Occurrence
{
    [JsonPropertyName("occurrenceType")]
    public string? OccurrenceType { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("recurringInterval")]
    public Interval? RecurringInterval { get; set; }

    [JsonPropertyName("event")]
    public string? Event { get; set; }

    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonPropertyName("eventName")]
    public string? EventName { get; set; }
}

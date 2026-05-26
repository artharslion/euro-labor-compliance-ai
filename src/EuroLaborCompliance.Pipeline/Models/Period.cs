namespace EuroLaborCompliance.Pipeline.Models;

public class Period
{
    [JsonPropertyName("datePeriod")]
    public DatePeriod[]? DatePeriod { get; init; }

    [JsonPropertyName("timePeriod")]
    public TimePeriod? TimePeriod { get; init; }

    [JsonPropertyName("weekday")]
    public WeekdayCode[]? Weekday { get; init; }
}

public class DatePeriod
{
    [JsonPropertyName("startDate")]
    public required string StartDate { get; init; }

    [JsonPropertyName("endDate")]
    public string? EndDate { get; init; }
}

public class TimePeriod
{
    [JsonPropertyName("startTime")]
    public string? StartTime { get; init; }

    [JsonPropertyName("endTime")]
    public string? EndTime { get; init; }
}

public class WorkDuration
{
    [JsonPropertyName("amount")]
    public required Amount Amount { get; init; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; init; }

    [JsonPropertyName("valuePerWeek")]
    public double? ValuePerWeek { get; init; }
}

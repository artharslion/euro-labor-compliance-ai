namespace EuroLaborCompliance.Pipeline.Models;

public class Period
{
    [JsonPropertyName("datePeriod")]
    public DatePeriod[]? DatePeriod { get; set; }

    [JsonPropertyName("timePeriod")]
    public TimePeriod? TimePeriod { get; set; }

    [JsonPropertyName("weekday")]
    public WeekdayCode[]? Weekday { get; set; }
}

public class DatePeriod
{
    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }
}

public class TimePeriod
{
    [JsonPropertyName("startTime")]
    public string? StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public string? EndTime { get; set; }
}

public class WorkDuration
{
    [JsonPropertyName("amount")]
    public Amount Amount { get; set; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; set; }

    [JsonPropertyName("valuePerWeek")]
    public double? ValuePerWeek { get; set; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class Amount
{
    [JsonPropertyName("value")]
    public required double Value { get; init; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; init; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; init; }

    [JsonPropertyName("unitCode")]
    public required AmountUnitCode UnitCode { get; init; }

    [JsonPropertyName("baseAmount")]
    public BaseAmount? BaseAmount { get; init; }

    [JsonPropertyName("proportional")]
    public Proportional? Proportional { get; init; }
}

public class BaseAmount
{
    [JsonPropertyName("unitCode")]
    public required BaseUnitCode UnitCode { get; init; }

    [JsonPropertyName("baseType")]
    public required BaseDefinitionCode BaseType { get; init; }

    [JsonPropertyName("value")]
    public double? Value { get; init; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; init; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; init; }
}

public class Proportional
{
    [JsonPropertyName("partTimePercentage")]
    public double? PartTimePercentage { get; init; }

    [JsonPropertyName("employmentDuration")]
    public double? EmploymentDuration { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

public class Interval
{
    [JsonPropertyName("value")]
    public required double Value { get; init; }

    [JsonPropertyName("unitCode")]
    public required IntervalCode UnitCode { get; init; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class Amount
{
    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }

    [JsonPropertyName("unitCode")]
    public AmountUnitCode UnitCode { get; set; }

    [JsonPropertyName("baseAmount")]
    public BaseAmount? BaseAmount { get; set; }

    [JsonPropertyName("proportional")]
    public Proportional? Proportional { get; set; }
}

public class BaseAmount
{
    [JsonPropertyName("unitCode")]
    public BaseUnitCode UnitCode { get; set; }

    [JsonPropertyName("baseType")]
    public BaseDefinitionCode BaseType { get; set; }

    [JsonPropertyName("value")]
    public double? Value { get; set; }

    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }
}

public class Proportional
{
    [JsonPropertyName("partTimePercentage")]
    public double? PartTimePercentage { get; set; }

    [JsonPropertyName("employmentDuration")]
    public double? EmploymentDuration { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class Interval
{
    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("unitCode")]
    public IntervalCode UnitCode { get; set; }
}

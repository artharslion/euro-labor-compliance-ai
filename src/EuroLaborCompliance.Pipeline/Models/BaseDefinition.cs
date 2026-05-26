namespace EuroLaborCompliance.Pipeline.Models;

public class BaseDefinition
{
    [JsonPropertyName("baseType")]
    public required BaseDefinitionCode BaseType { get; init; }

    [JsonPropertyName("remunerationIndicator")]
    public required bool RemunerationIndicator { get; init; }

    [JsonPropertyName("holidayAllowanceIndicator")]
    public required bool HolidayAllowanceIndicator { get; init; }

    [JsonPropertyName("paidLeaveDayIndicator")]
    public required bool PaidLeaveDayIndicator { get; init; }

    [JsonPropertyName("allAllowancesIndicator")]
    public required bool AllAllowancesIndicator { get; init; }

    [JsonPropertyName("allowances")]
    public AllowanceCode[]? Allowances { get; init; }

    [JsonPropertyName("referenceDate")]
    public string? ReferenceDate { get; init; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class BaseDefinition
{
    [JsonPropertyName("baseType")]
    public BaseDefinitionCode BaseType { get; set; }

    [JsonPropertyName("remunerationIndicator")]
    public bool RemunerationIndicator { get; set; }

    [JsonPropertyName("holidayAllowanceIndicator")]
    public bool HolidayAllowanceIndicator { get; set; }

    [JsonPropertyName("paidLeaveDayIndicator")]
    public bool PaidLeaveDayIndicator { get; set; }

    [JsonPropertyName("allAllowancesIndicator")]
    public bool AllAllowancesIndicator { get; set; }

    [JsonPropertyName("allowances")]
    public AllowanceCode[]? Allowances { get; set; }

    [JsonPropertyName("referenceDate")]
    public string? ReferenceDate { get; set; }
}

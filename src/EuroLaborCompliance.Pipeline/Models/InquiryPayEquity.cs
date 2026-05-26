namespace EuroLaborCompliance.Pipeline.Models;

public class InquiryPayEquity
{
    [JsonPropertyName("documentId")]
    public required Identifier DocumentId { get; init; }

    [JsonPropertyName("versionId")]
    public required string VersionId { get; init; }

    [JsonPropertyName("issued")]
    public required string Issued { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public required EffectivePeriod EffectivePeriod { get; init; }

    [JsonPropertyName("customer")]
    public required Party Customer { get; init; }

    [JsonPropertyName("baseDefinition")]
    public required BaseDefinition[] BaseDefinition { get; init; }

    [JsonPropertyName("labourAgreements")]
    public LabourAgreements? LabourAgreements { get; init; }

    [JsonPropertyName("positionProfile")]
    public PositionProfile[]? PositionProfile { get; init; }

    [JsonPropertyName("remuneration")]
    public RemunerationPackage[]? Remuneration { get; init; }

    [JsonPropertyName("allowance")]
    public AllowanceArrangement[]? Allowance { get; init; }

    [JsonPropertyName("holidayAllowance")]
    public AllowanceArrangement[]? HolidayAllowance { get; init; }

    [JsonPropertyName("sickPay")]
    public AllowanceArrangement[]? SickPay { get; init; }

    [JsonPropertyName("leave")]
    public LeaveArrangement[]? Leave { get; init; }

    [JsonPropertyName("individualChoiceBudget")]
    public AllowanceArrangement[]? IndividualChoiceBudget { get; init; }

    [JsonPropertyName("pension")]
    public PensionArrangement[]? Pension { get; init; }

    [JsonPropertyName("sustainableEmployability")]
    public AllowanceArrangement[]? SustainableEmployability { get; init; }

    [JsonPropertyName("supplementaryArrangement")]
    public SupplementaryArrangement[]? SupplementaryArrangement { get; init; }

    [JsonPropertyName("otherArrangement")]
    public AllowanceArrangement[]? OtherArrangement { get; init; }
}

public class SupplementaryArrangement
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("origin")]
    public string? Origin { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }

    [JsonPropertyName("typeCode")]
    public required SupplementaryArrangementCode TypeCode { get; init; }

    [JsonPropertyName("line")]
    public ArrangementLine[]? Line { get; init; }

    [JsonPropertyName("phaseOutScheme")]
    public PhaseOutScheme? PhaseOutScheme { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class InquiryPayEquity
{
    [JsonPropertyName("documentId")]
    public Identifier DocumentId { get; set; }

    [JsonPropertyName("versionId")]
    public string VersionId { get; set; }

    [JsonPropertyName("issued")]
    public string Issued { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod EffectivePeriod { get; set; }

    [JsonPropertyName("customer")]
    public Party Customer { get; set; }

    [JsonPropertyName("baseDefinition")]
    public BaseDefinition[] BaseDefinition { get; set; }

    [JsonPropertyName("labourAgreements")]
    public LabourAgreements? LabourAgreements { get; set; }

    [JsonPropertyName("positionProfile")]
    public PositionProfile[]? PositionProfile { get; set; }

    [JsonPropertyName("remuneration")]
    public RemunerationPackage[]? Remuneration { get; set; }

    [JsonPropertyName("allowance")]
    public AllowanceArrangement[]? Allowance { get; set; }

    [JsonPropertyName("holidayAllowance")]
    public AllowanceArrangement[]? HolidayAllowance { get; set; }

    [JsonPropertyName("sickPay")]
    public AllowanceArrangement[]? SickPay { get; set; }

    [JsonPropertyName("leave")]
    public LeaveArrangement[]? Leave { get; set; }

    [JsonPropertyName("individualChoiceBudget")]
    public AllowanceArrangement[]? IndividualChoiceBudget { get; set; }

    [JsonPropertyName("pension")]
    public PensionArrangement[]? Pension { get; set; }

    [JsonPropertyName("sustainableEmployability")]
    public AllowanceArrangement[]? SustainableEmployability { get; set; }

    [JsonPropertyName("supplementaryArrangement")]
    public SupplementaryArrangement[]? SupplementaryArrangement { get; set; }

    [JsonPropertyName("otherArrangement")]
    public AllowanceArrangement[]? OtherArrangement { get; set; }
}

public class SupplementaryArrangement
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("origin")]
    public string? Origin { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }

    [JsonPropertyName("typeCode")]
    public SupplementaryArrangementCode TypeCode { get; set; }

    [JsonPropertyName("line")]
    public ArrangementLine[]? Line { get; set; }

    [JsonPropertyName("phaseOutScheme")]
    public PhaseOutScheme? PhaseOutScheme { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

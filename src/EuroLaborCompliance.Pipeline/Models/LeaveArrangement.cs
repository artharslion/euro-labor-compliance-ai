namespace EuroLaborCompliance.Pipeline.Models;

public class LeaveArrangement
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("origin")]
    public string? Origin { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }

    [JsonPropertyName("paidLeave")]
    public PaidLeave[]? PaidLeave { get; set; }

    [JsonPropertyName("holidays")]
    public Holiday[]? Holidays { get; set; }

    [JsonPropertyName("specialLeave")]
    public SpecialLeave[]? SpecialLeave { get; set; }

    [JsonPropertyName("additionalParentalLeave")]
    public AdditionalParentalLeave[]? AdditionalParentalLeave { get; set; }

    [JsonPropertyName("mandatoryLeaveAllocation")]
    public MandatoryLeaveAllocation? MandatoryLeaveAllocation { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class PaidLeave
{
    [JsonPropertyName("leaveType")]
    public string LeaveType { get; set; }

    [JsonPropertyName("leaveQuantity")]
    public Amount? LeaveQuantity { get; set; }

    [JsonPropertyName("leaveMeasurement")]
    public string? LeaveMeasurement { get; set; }
}

public class Holiday
{
    [JsonPropertyName("holidayCode")]
    public string? HolidayCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class SpecialLeave
{
    [JsonPropertyName("leaveType")]
    public string LeaveType { get; set; }

    [JsonPropertyName("leaveQuantity")]
    public Amount? LeaveQuantity { get; set; }
}

public class AdditionalParentalLeave
{
    [JsonPropertyName("leaveType")]
    public string LeaveType { get; set; }

    [JsonPropertyName("leaveQuantity")]
    public Amount? LeaveQuantity { get; set; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; set; }
}

public class MandatoryLeaveAllocation
{
    [JsonPropertyName("allocationType")]
    public string AllocationType { get; set; }

    [JsonPropertyName("allocationQuantity")]
    public Amount? AllocationQuantity { get; set; }
}

public class PensionArrangement
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("origin")]
    public string? Origin { get; set; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; set; }

    [JsonPropertyName("line")]
    public PensionLine[]? Line { get; set; }

    [JsonPropertyName("franchise")]
    public Amount? Franchise { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public class PensionLine
{
    [JsonPropertyName("lineId")]
    public string LineId { get; set; }

    [JsonPropertyName("amount")]
    public Amount Amount { get; set; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; set; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; set; }

    [JsonPropertyName("contributionSource")]
    public ContributionSource? ContributionSource { get; set; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class LeaveArrangement
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("origin")]
    public string? Origin { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }

    [JsonPropertyName("paidLeave")]
    public PaidLeave[]? PaidLeave { get; init; }

    [JsonPropertyName("holidays")]
    public Holiday[]? Holidays { get; init; }

    [JsonPropertyName("specialLeave")]
    public SpecialLeave[]? SpecialLeave { get; init; }

    [JsonPropertyName("additionalParentalLeave")]
    public AdditionalParentalLeave[]? AdditionalParentalLeave { get; init; }

    [JsonPropertyName("mandatoryLeaveAllocation")]
    public MandatoryLeaveAllocation? MandatoryLeaveAllocation { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

public class PaidLeave
{
    [JsonPropertyName("leaveType")]
    public required string LeaveType { get; init; }

    [JsonPropertyName("leaveQuantity")]
    public Amount? LeaveQuantity { get; init; }

    [JsonPropertyName("leaveMeasurement")]
    public string? LeaveMeasurement { get; init; }
}

public class Holiday
{
    [JsonPropertyName("holidayCode")]
    public string? HolidayCode { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

public class SpecialLeave
{
    [JsonPropertyName("leaveType")]
    public required string LeaveType { get; init; }

    [JsonPropertyName("leaveQuantity")]
    public Amount? LeaveQuantity { get; init; }
}

public class AdditionalParentalLeave
{
    [JsonPropertyName("leaveType")]
    public required string LeaveType { get; init; }

    [JsonPropertyName("leaveQuantity")]
    public Amount? LeaveQuantity { get; init; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; init; }
}

public class MandatoryLeaveAllocation
{
    [JsonPropertyName("allocationType")]
    public required string AllocationType { get; init; }

    [JsonPropertyName("allocationQuantity")]
    public Amount? AllocationQuantity { get; init; }
}

public class PensionArrangement
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("origin")]
    public string? Origin { get; init; }

    [JsonPropertyName("effectivePeriod")]
    public EffectivePeriod? EffectivePeriod { get; init; }

    [JsonPropertyName("line")]
    public PensionLine[]? Line { get; init; }

    [JsonPropertyName("franchise")]
    public Amount? Franchise { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }
}

public class PensionLine
{
    [JsonPropertyName("lineId")]
    public required string LineId { get; init; }

    [JsonPropertyName("amount")]
    public required Amount Amount { get; init; }

    [JsonPropertyName("interval")]
    public Interval? Interval { get; init; }

    [JsonPropertyName("conditions")]
    public Condition[]? Conditions { get; init; }

    [JsonPropertyName("contributionSource")]
    public ContributionSource? ContributionSource { get; init; }
}

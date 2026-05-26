namespace EuroLaborCompliance.Pipeline.Models;

public class Party
{
    [JsonPropertyName("id")]
    public required Identifier[] Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("legalId")]
    public IdValue[]? LegalId { get; init; }

    [JsonPropertyName("personContacts")]
    public ContactPerson[]? PersonContacts { get; init; }
}

public class ContactPerson
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("communication")]
    public Communication? Communication { get; init; }

    [JsonPropertyName("roleCode")]
    public string? RoleCode { get; init; }

    [JsonPropertyName("positionTitle")]
    public string? PositionTitle { get; init; }
}

public class Communication
{
    [JsonPropertyName("phone")]
    public PhoneNumber[]? Phone { get; init; }

    [JsonPropertyName("email")]
    public EmailAddress[]? Email { get; init; }
}

public class PhoneNumber
{
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

public class EmailAddress
{
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

namespace EuroLaborCompliance.Pipeline.Models;

public class Party
{
    [JsonPropertyName("id")]
    public Identifier[] Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("legalId")]
    public IdValue[]? LegalId { get; set; }

    [JsonPropertyName("personContacts")]
    public ContactPerson[]? PersonContacts { get; set; }
}

public class ContactPerson
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("communication")]
    public Communication? Communication { get; set; }

    [JsonPropertyName("roleCode")]
    public string? RoleCode { get; set; }

    [JsonPropertyName("positionTitle")]
    public string? PositionTitle { get; set; }
}

public class Communication
{
    [JsonPropertyName("phone")]
    public PhoneNumber[]? Phone { get; set; }

    [JsonPropertyName("email")]
    public EmailAddress[]? Email { get; set; }
}

public class PhoneNumber
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class EmailAddress
{
    [JsonPropertyName("value")]
    public string Value { get; set; }
}

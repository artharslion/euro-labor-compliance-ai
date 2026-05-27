using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using EuroLaborCompliance.Pipeline.Models;

namespace EuroLaborCompliance.Pipeline.Mapping;

public class LlmMapper : IMapper
{
    private readonly HttpClient _http;
    private const string ApiUrl = "https://opencode.ai/zen/go/v1/chat/completions";
    private const string Model = "deepseek-v4-pro";
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = false
    };

    public LlmMapper(string apiKey, HttpClient? http = null)
    {
        _http = http ?? new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<MappingResult> MapAsync(string markdown, string sourceFile)
    {
        var flags = new List<MappingFlag>();
        var prompt = BuildPrompt(markdown);

        try
        {
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = prompt }
                },
                max_tokens = 16384,
                temperature = 0.1
            };

            var json = JsonSerializer.Serialize(requestBody, JsonOpts);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(ApiUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                var errBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"API returned {(int)response.StatusCode}: {errBody[..Math.Min(200, errBody.Length)]}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(responseJson);

            // Extract content from OpenAI-compatible response
            var choices = doc.RootElement.GetProperty("choices");
            var message = choices[0].GetProperty("message");
            var llmOutput = message.GetProperty("content").GetString() ?? "";

            // Extract JSON from LLM output (may be wrapped in markdown code fences)
            var cleanJson = ExtractJson(llmOutput);

            // Parse with lenient options
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            InquiryPayEquity output;
            try
            {
                output = JsonSerializer.Deserialize<InquiryPayEquity>(cleanJson, jsonOptions)!;
            }
            catch (JsonException)
            {
                // LLM might return simplified format — try manual mapping from JsonDocument
                output = MapFromJsonDocument(cleanJson);
            }

            if (output == null)
            {
                flags.Add(new MappingFlag("llm", "error", "LLM returned invalid JSON"));
                var fallback = await new SemanticMapper().MapAsync(markdown, sourceFile);
                return new MappingResult(fallback.Output, fallback.OverallConfidence,
                    fallback.FieldsMapped, fallback.FieldsMissing,
                    flags.Concat(fallback.Flags).ToList());
            }

            // Post-process: fill in generated fields
            output.DocumentId = new Identifier { Value = Guid.NewGuid().ToString(), SchemeAgencyId = "Customer" };
            output.VersionId = "v1.0-llm";
            output.Issued = DateTime.UtcNow.ToString("o");

            var confidence = EstimateConfidence(cleanJson);
            return new MappingResult(output, confidence, 15, 0, flags);
        }
        catch (Exception ex)
        {
            flags.Add(new MappingFlag("llm", "error", $"API: {ex.Message}"));
            var fallback = await new SemanticMapper().MapAsync(markdown, sourceFile);
            return new MappingResult(fallback.Output, fallback.OverallConfidence,
                fallback.FieldsMapped, fallback.FieldsMissing,
                flags.Concat(fallback.Flags).ToList());
        }
    }

    private static string BuildPrompt(string markdown)
    {
        var truncated = markdown.Length > 30000 ? markdown[..30000] + "\n\n[TRUNCATED]" : markdown;

        return SystemPrompt + "\n\n" + """
            Extract ALL employment and remuneration data from the following Dutch labor document.
            Output a complete SETU InquiryPayEquity v2.0 JSON object with ALL available fields.

            TOP-LEVEL REQUIRED FIELDS:
            - documentId: {"value":"auto","schemeAgencyId":"Customer"}
            - effectivePeriod: {"validFrom":"2026-01-01","validTo":"2027-12-31"}
            - customer: {name, legalId:[{value, schemeAgencyId:"KvK"}], personContacts:[{name, communication:{email:[{address, useCode}], phone:[{number}]}, roleCode, positionTitle}]}

            COMPLETE FIELD LIST (include ALL that you find in the document):

            1. remuneration[] — salary packages. For EACH:
               - origin: {"type":"CollectiveLabourAgreement"}
               - workDuration: {amount:{value, unitCode:"Hour"}, interval:{value, unitCode:"Week"}, valuePerWeek}
               - hourlyWageConversion: {hourlyWageFactor:173.33, hourlyWagePercentage:0.607}
               - salaryScale[] — FULL salary table with ALL steps:
                 - name, minValue, maxValue, currency:"EUR"
                 - salaryStep[]: EACH with {name, value, minimumWage:false, conditions:[...]}
                 - positionProfileReference:[{positionId, startSalaryStep}]
               - individualSalaryIncrease[]: {effectiveDate, line:{amount:{value,unitCode:"Percentage",baseAmount:{unitCode:"HourlyRate",baseType:"GrossSalary"}}}}
               - generalSalaryIncrease[]: {effectiveDate, line:{amount:{value,unitCode:"Percentage"...}}}
               - conditions[]: when this package applies

            2. positionProfile[] — job functions:
               - positionId, positionTitle, origin, referenceTitle, workDescription

            3. allowance[] — ALL surcharges and compensations. For EACH:
               - id, name, origin, typeCode (from list below)
               - period[]: {datePeriod:[{start,end}], timePeriod:{start,end}, weekday:[...]}
               - line[]: {lineId, amount:{value,unitCode,baseAmount:{unitCode,baseType}}, interval:{value,unitCode}, conditions:[...]}
               - payDate, description

            4. holidayAllowance[]:
               - line: {amount:{value:8.33,unitCode:"Percentage",baseAmount:{unitCode:"YearlyRate",baseType:"GrossSalary"}}}
               - payDate

            5. sickPay[]:
               - waitingDays:{value,unitCode:"Day"}
               - line[]: EACH tier {amount:{value,unitCode:"Percentage",baseAmount:{...}}, interval, conditions:[{conditionType:"Occurrence",occurrence:{occurrenceType:"Relative",event:"SickLeave",offset:"P0D"}}]}

            6. leave[] — ALL leave types:
               - paidLeave[]: {name, lineId, amount:{value,unitCode:"Day"}, interval:{value:1,unitCode:"Year"}}
               - holidays[]: {name, lineId, amount:{value,unitCode:"Day"}}
               - specialLeave[]: {name, lineId, amount:{value,unitCode:"Day"}}
               - additionalParentalLeave[]: {name, lineId, amount:{value,unitCode:"Day"}}

            7. pension[]:
               - line[]: EACH {lineId,amount:{value,unitCode:"Percentage",baseAmount:{unitCode:"HourlyRate",baseType:"GrossSalary"}},contributionSource:{type:"Employer"|"Employee"}}
               - franchise:{description:"Franchise per hour: E9.24"}

            8. individualChoiceBudget[]: {id, name, origin, line[]}
            9. sustainableEmployability[]: {id, name, origin, typeCode:"Education", line:[{amount:{value,unitCode:"Euro",baseAmount:{unitCode:"Fixed"}}, interval:{value:1,unitCode:"Year"}}]}
            10. supplementaryArrangement[]: {id, name, origin, typeCode, line[], description}
            11. baseDefinition[]: {baseType:"GrossSalary", remunerationIndicator:true, holidayAllowanceIndicator:true, paidLeaveDayIndicator:true, allAllowancesIndicator:true}
            12. labourAgreements: {industryIdentifier:[{value}], collectiveLabourAgreement:{name,id:{value},typeCode:"CollectiveLabourAgreement"}, customLabourAgreement:false}

            ALLOWANCE CODE REFERENCE:
            Overtime: HT100(mon-sat), HT102(sunday) | Irregular hours: HT200, HT201 | Weekend: HT321, Sat:HT322, Sun:HT330
            Night: HT340 | Holiday: HT331 | Shift: HT300, HT301, HT302 | Hazard: HT600
            Commuting: EA103 | Home office: EA801 | Internet: EA606 | Phone: EA605

            CONDITION TYPES for salary steps and allowances:
            - Age: {conditionType:"Age", operator:"gte"|"lte"|"eq", age:number}
            - EmploymentDuration: {conditionType:"EmploymentDuration", operator:"gte", duration:"P1Y", referenceDateType:"HireDate"}
            - PositionProfile: {conditionType:"PositionProfile", operator:"in", positionProfileIds:[...]}
            - AllOf: {conditionType:"AllOf", conditions:[...]}
            - Text: {conditionType:"Text", description:"..."}

            NUMERIC VALUES: Extract exact numbers from the document. Use commas for Dutch decimal notation (15,31 → 15.31).
            If a field is not found, OMIT it entirely. Do NOT invent values.

            DOCUMENT TEXT:
            """ + truncated + """

            Output ONLY valid JSON, no markdown fences, no explanations.
            """;
    }

    private static string SystemPrompt => """
        You are a Dutch labor compliance data extraction specialist.
        You extract structured remuneration data from Collective Labour Agreement (CAO) documents
        and output SETU InquiryPayEquity v2.0 compliant JSON.

        Key knowledge:
        - Dutch "toeslag" = surcharge/allowance → AllowanceCode starting with HT
        - Dutch "vergoeding" = reimbursement → AllowanceCode starting with EA
        - "vakantiebijslag" = holiday allowance, typically 8.33% of gross annual salary
        - "vakantiedagen" = vacation days, minimum 20, typically 25
        - "pensioen" = pension, employer + employee contributions
        - "IKB" = Individueel Keuze Budget (flexible benefits budget)
        - CAO = Collectieve Arbeidsovereenkomst (collective labor agreement)
        - "functiegroep" = function group / job classification
        - "periodiek" = periodic salary increase
        - "overwerk" = overtime
        - "ploegendienst" = shift work
        - "onregelmatige uren" = irregular hours
        - "wachtdagen" = waiting days (for sick pay)

        Output format: raw JSON matching the SETU InquiryPayEquity v2.0 schema.
        Use camelCase property names. Skip fields not found in the document.
        """;

    private static string ExtractJson(string llmOutput)
    {
        var trimmed = llmOutput.Trim();
        // Strip markdown code fences if present
        if (trimmed.StartsWith("```"))
        {
            var start = trimmed.IndexOf('\n') + 1;
            var end = trimmed.LastIndexOf("```");
            if (end > start) trimmed = trimmed[start..end].Trim();
        }
        return trimmed;
    }

    private static double EstimateConfidence(string json)
    {
        int score = 0;
        if (json.Contains("\"customer\"")) score += 2;
        if (json.Contains("\"remuneration\"")) score += 3;
        if (json.Contains("\"allowance\"")) score += 2;
        if (json.Contains("\"holidayAllowance\"")) score += 1;
        if (json.Contains("\"sickPay\"")) score += 1;
        if (json.Contains("\"leave\"")) score += 1;
        if (json.Contains("\"pension\"")) score += 2;
        if (json.Contains("\"salaryStep\"")) score += 2;
        return Math.Min(0.95, score / 15.0 + 0.5);
    }

    private static InquiryPayEquity MapFromJsonDocument(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var result = new InquiryPayEquity
        {
            DocumentId = new Identifier { Value = Guid.NewGuid().ToString(), SchemeAgencyId = "Customer" },
            VersionId = "v1.0-llm",
            Issued = DateTime.UtcNow.ToString("o"),
            EffectivePeriod = new EffectivePeriod { ValidFrom = "2026-01-01", ValidTo = "2027-12-31" }
        };

        // customer
        if (root.TryGetProperty("customer", out var cust))
        {
            result.Customer = new Party
            {
                Name = cust.TryGetPropStr("name"),
                LegalId = JsonElementExt.TryGetArray(cust, "legalId", out var lids)
                    ? lids.Select(l => new IdValue { Value = l.TryGetPropStr("value") ?? l.TryGetPropStr("kvk") ?? l.GetRawText().Trim('"') }).ToArray()
                    : null,
                PersonContacts = JsonElementExt.TryGetArray(cust, "personContacts", out var contacts)
                    ? contacts.Select(c => new ContactPerson
                    {
                        Name = c.TryGetPropStr("name") ?? c.TryGetPropStr("formattedName"),
                        Communication = new Communication
                        {
                            Email = c.TryGetPropStr("email") is { } e ? new[] { new EmailAddress { Value = e } } : null,
                            Phone = c.TryGetPropStr("phone") is { } p ? new[] { new PhoneNumber { Value = p } } : null
                        },
                        RoleCode = c.TryGetPropStr("roleCode"),
                        PositionTitle = c.TryGetPropStr("positionTitle")
                    }).ToArray()
                    : null
            };
        }

        // labourAgreements
        if (root.TryGetProperty("labourAgreements", out var la))
        {
            result.LabourAgreements = new LabourAgreements
            {
                IndustryIdentifier = JsonElementExt.TryGetArray(la, "industryIdentifier", out var ii)
                    ? ii.Select(i => new Identifier { Value = i.TryGetPropStr("value") ?? "", SchemeAgencyId = "CBS" }).ToArray() : null,
                CollectiveLabourAgreement = la.TryGetProperty("collectiveLabourAgreement", out var cla)
                    ? new LabourAgreement { Name = cla.TryGetPropStr("name"), TypeCode = LabourAgreementType.CollectiveLabourAgreement }
                    : null,
                CustomLabourAgreement = la.TryGetPropStr("customLabourAgreement") is { } caStr && caStr.Equals("true", StringComparison.OrdinalIgnoreCase)
                    ? new LabourAgreement { Name = "Custom", TypeCode = LabourAgreementType.CustomLabourAgreement } : null
            };
        }

        // positionProfile
        if (JsonElementExt.TryGetArray(root, "positionProfile", out var pp))
        {
            result.PositionProfile = pp.Select(p => new PositionProfile
            {
                PositionId = p.TryGetPropStr("positionId"),
                PositionTitle = p.TryGetPropStr("positionTitle"),
                Origin = p.TryGetPropStr("origin") ?? "CollectiveLabourAgreement",
                ReferenceTitle = p.TryGetPropStr("referenceTitle"),
                WorkDescription = p.TryGetPropStr("workDescription")
            }).ToArray();
        }

        // remuneration
        if (JsonElementExt.TryGetArray(root, "remuneration", out var rems))
        {
            result.Remuneration = rems.Select(r => new RemunerationPackage
            {
                Origin = r.TryGetPropStr("origin") ?? "CollectiveLabourAgreement",
                WorkDuration = r.TryGetProperty("workDuration", out var wd) ? new WorkDuration
                {
                    Amount = new Amount { Value = wd.TryGetPropNum("value"), UnitCode = AmountUnitCode.Hour },
                    ValuePerWeek = wd.TryGetPropNum("valuePerWeek")
                } : null,
                SalaryScale = JsonElementExt.TryGetArray(r, "salaryScale", out var scales)
                    ? scales.Select(s => new SalaryScale
                    {
                        Name = s.TryGetPropStr("name"),
                        MinValue = s.TryGetPropNum("minValue"),
                        MaxValue = s.TryGetPropNum("maxValue"),
                        Currency = s.TryGetPropStr("currency") ?? "EUR",
                        SalaryStep = JsonElementExt.TryGetArray(s, "salaryStep", out var steps)
                            ? steps.Select(st => new SalaryScaleStep
                            {
                                Name = st.TryGetPropStr("name"),
                                Value = st.TryGetPropNum("value")
                            }).ToArray() : null
                    }).ToArray() : null
            }).ToArray();
        }

        // allowance
        if (JsonElementExt.TryGetArray(root, "allowance", out var allowances))
        {
            result.Allowance = allowances.Select(a => new AllowanceArrangement
            {
                Id = a.TryGetPropStr("id") ?? Guid.NewGuid().ToString()[..11],
                Name = a.TryGetPropStr("name"),
                Origin = a.TryGetPropStr("origin") ?? "CollectiveLabourAgreement",
                TypeCode = Enum.TryParse<AllowanceCode>(a.TryGetPropStr("typeCode") ?? "EA900", out var ac) ? ac : AllowanceCode.EA900,
                Line = JsonElementExt.TryGetArray(a, "line", out var lines)
                    ? lines.Select(l => new ArrangementLine
                    {
                        LineId = l.TryGetPropStr("lineId") ?? Guid.NewGuid().ToString()[..8],
                        Amount = new Amount { Value = l.TryGetPropNum("value"), UnitCode = AmountUnitCode.Percentage }
                    }).ToArray() : null
            }).ToArray();
        }

        return result;
    }
}

internal static class JsonElementExt
{
    public static string? TryGetPropStr(this JsonElement e, string name)
        => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    public static double TryGetPropNum(this JsonElement e, string name)
        => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : 0;

    public static bool TryGetArray(this JsonElement e, string name, out JsonElement.ArrayEnumerator arr)
    {
        if (e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Array)
        {
            arr = v.EnumerateArray();
            return arr.Any();
        }
        arr = default;
        return false;
    }
}

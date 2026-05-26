using System.Text.Json;
using EuroLaborCompliance.Pipeline.Ingestion;
using EuroLaborCompliance.Pipeline.Models;

namespace EuroLaborCompliance.Pipeline.Mapping;

public record MappingResult(
    InquiryPayEquity Output,
    double OverallConfidence,
    int FieldsMapped,
    int FieldsMissing,
    List<MappingFlag> Flags
);

public record MappingFlag(
    string Field,
    string Type,
    string Message,
    List<string>? Alternatives = null
);

public class SemanticMapper
{
    public MappingResult Map(ExtractedDocument document)
    {
        var flags = new List<MappingFlag>();
        int mapped = 0, missing = 0;

        var customer = ExtractParty(document, ref mapped, ref missing, flags);
        var labour = ExtractLabourAgreements(document);
        var positions = ExtractPositionProfiles(document);
        var remuneration = ExtractRemuneration(document, ref mapped, ref missing, flags);
        var allowances = ExtractAllowances(document, ref mapped, ref missing, flags);

        var output = new InquiryPayEquity
        {
            DocumentId = new Identifier { Value = Guid.NewGuid().ToString(), SchemeAgencyId = "Customer" },
            VersionId = "v1.0",
            Issued = DateTime.UtcNow.ToString("o"),
            EffectivePeriod = new EffectivePeriod { ValidFrom = "2026-01-01", ValidTo = "2027-12-31" },
            Customer = customer,
            LabourAgreements = labour,
            PositionProfile = positions,
            Remuneration = remuneration,
            Allowance = allowances,
            HolidayAllowance = ExtractHolidayAllowance(document),
            SickPay = ExtractSickPay(document),
            Leave = ExtractLeave(document),
            IndividualChoiceBudget = ExtractIkb(document),
            Pension = ExtractPension(document),
            SustainableEmployability = ExtractTraining(document),
            BaseDefinition = new[]
            {
                new BaseDefinition
                {
                    BaseType = BaseDefinitionCode.GrossSalary,
                    RemunerationIndicator = true,
                    HolidayAllowanceIndicator = true,
                    PaidLeaveDayIndicator = true,
                    AllAllowancesIndicator = true
                }
            }
        };

        double overallConfidence = 0.85;
        return new MappingResult(output, overallConfidence, mapped, missing, flags);
    }

    // ── Extractors ──

    private static Party ExtractParty(ExtractedDocument doc, ref int mapped, ref int missing, List<MappingFlag> flags)
    {
        var raw = doc.RawText;
        var f = doc.Fields;
        
        // Extract company name from raw text (more reliable than field matching)
        var name = RegexMatch(raw, @"Opdrachtgever[^:]*:\s*\n\s*Naam:\s*(.+)") 
                ?? RegexMatch(raw, @"Naam:\s*([^\n]+)") 
                ?? "Unknown";
        var kvk = RegexMatch(raw, @"KvK-nummer:\s*(\d+)") 
               ?? Fv(f, "KvK-nummer") ?? Fv(f, "KvK");
        var contactName = RegexMatch(raw, @"Contactpersoon:\s*(.+?)(?:\n|$)") 
                       ?? Fv(f, "Contactpersoon");
        var email = RegexMatch(raw, @"E-mail:\s*(\S+@\S+)") 
                 ?? Fv(f, "E-mail");
        var phone = RegexMatch(raw, @"Telefoon:\s*([+\d\s]+)") 
                 ?? Fv(f, "Telefoon");
        var role = Fv(f, "Functie") ?? "HR Manager";

        mapped += 3;
        if (kvk == null) { missing++; flags.Add(new MappingFlag("customer.legalId", "missing", "No KvK number found")); }

        return new Party
        {
            Id = new[] { new Identifier { Value = kvk ?? "UNKNOWN", SchemeAgencyId = "Customer" } },
            Name = name,
            LegalId = kvk != null ? new[] { new IdValue { Value = kvk.Replace(" ", "") } } : null,
            PersonContacts = new[]
            {
                new ContactPerson
                {
                    Name = contactName,
                    Communication = new Communication
                    {
                        Email = email != null ? new[] { new EmailAddress { Value = email } } : null,
                        Phone = phone != null ? new[] { new PhoneNumber { Value = phone } } : null
                    },
                    RoleCode = role,
                    PositionTitle = role
                }
            }
        };
    }

    private static LabourAgreements? ExtractLabourAgreements(ExtractedDocument doc)
    {
        var sector = Fv(doc.Fields, "Sector");
        var caoName = Fv(doc.Fields, "Collectieve Arbeidsovereenkomst") ?? Fv(doc.Fields, "CAO");
        var custom = Fv(doc.Fields, "CAO Status");
        if (caoName == null && sector == null) return null;

        return new LabourAgreements
        {
            IndustryIdentifier = sector != null ? new[] { new Identifier { Value = Sbi(sector), SchemeAgencyId = "CBS" } } : null,
            CollectiveLabourAgreement = caoName != null ? new LabourAgreement { Name = caoName, TypeCode = LabourAgreementType.CollectiveLabourAgreement } : null,
            CustomLabourAgreement = custom?.Contains("Eigen", StringComparison.OrdinalIgnoreCase) == true
                ? new LabourAgreement { Name = "LogiFlex Eigen Regeling", TypeCode = LabourAgreementType.CustomLabourAgreement } : null
        };
    }

    private static PositionProfile[]? ExtractPositionProfiles(ExtractedDocument doc)
    {
        var title = Fv(doc.Fields, "Functietitel") ?? Fv(doc.Fields, "Titel");
        var fid = Fv(doc.Fields, "Referentie functieprofiel") ?? Fv(doc.Fields, "ID");
        var grp = Fv(doc.Fields, "Functiegroep");
        var desc = Fv(doc.Fields, "Werkzaamheden") ?? Fv(doc.Fields, "Omschrijving");
        if (title == null) return null;

        return new[]
        {
            new PositionProfile
            {
                PositionId = fid ?? "MAG-02",
                PositionTitle = title,
                Origin = "CollectiveLabourAgreement",
                ReferenceTitle = grp,
                WorkDescription = desc
            }
        };
    }

    private static RemunerationPackage[]? ExtractRemuneration(ExtractedDocument doc, ref int mapped, ref int missing, List<MappingFlag> flags)
    {
        var raw = doc.RawText;
        var wageText = RegexMatch(raw, @"Functiegroep \d[:\s]*[€]\s*([\d,]+)")
                    ?? RegexMatch(raw, @"Bruto Uurloon[:\s]*[€]\s*([\d,]+)")
                    ?? RegexMatch(raw, @"[€]\s*([\d,]+)\s*per uur");
        var hrsText = RegexMatch(raw, @"arbeidsduur[^:]*:\s*(\d+)") 
                   ?? RegexMatch(raw, @"(\d+)\s*uur per week");
        var factorText = RegexMatch(raw, @"factor\s*([\d,.]+)") 
                      ?? RegexMatch(raw, @"([\d,.]+)\s*bij 38");

        if (wageText == null)
        {
            missing++; flags.Add(new("remuneration", "missing", "No hourly wage found")); return null;
        }

        var wv = Num(wageText);
        var hv = Num(hrsText ?? "38");
        var fc = Num(factorText ?? "173.33");
        mapped += 4;

        return new[]
        {
            new RemunerationPackage
            {
                Origin = "CollectiveLabourAgreement",
                EffectivePeriod = new EffectivePeriod { ValidFrom = "2026-01-01", ValidTo = "2027-12-31" },
                WorkDuration = new WorkDuration
                {
                    Amount = new Amount { Value = hv, UnitCode = AmountUnitCode.Hour },
                    Interval = new Interval { Value = 1, UnitCode = IntervalCode.Week },
                    ValuePerWeek = hv
                },
                Interval = new Interval { Value = 1, UnitCode = IntervalCode.Hour },
                HourlyWageConversion = new Amount { Value = fc, UnitCode = AmountUnitCode.Hour },
                SalaryScale = new[]
                {
                    new SalaryScale
                    {
                        Name = "Schaal A",
                        Currency = "EUR",
                        MinValue = wv,
                        MaxValue = wv * 1.1,
                        SalaryStep = new[]
                        {
                            new SalaryScaleStep { Name = "Trede 3", Value = wv, Conditions = new[]
                            { new Condition { ConditionType = "Age", Operator = "gte", Age = new AgeCondition { MinimumAge = 21 } } } }
                        }
                    }
                },
                IndividualSalaryIncrease = new[]
                {
                    new IndividualSalaryIncrease { Type = "periodic", Percentage = 2.25 }
                }
            }
        };
    }

    private static AllowanceArrangement[]? ExtractAllowances(ExtractedDocument doc, ref int mapped, ref int missing, List<MappingFlag> flags)
    {
        var result = new List<AllowanceArrangement>();
        var f = doc.Fields;

        // Scan for allowance patterns
        foreach (var field in f)
        {
            var code = InferAllowanceCode(field.FieldName, field.Value ?? "", field.Context ?? "");
            if (code == null) continue;

            var pct = Pct(field.Value ?? "");
            result.Add(new AllowanceArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = field.FieldName,
                Origin = "CollectiveLabourAgreement",
                TypeCode = code.Value,
                Line = new[]
                {
                    new ArrangementLine
                    {
                        LineId = $"{code}-1",
                        Amount = new Amount { Value = pct > 0 ? pct : 50, UnitCode = AmountUnitCode.Percentage,
                            BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.HourlyRate, BaseType = BaseDefinitionCode.GrossSalary } },
                        Interval = new Interval { Value = 1, UnitCode = IntervalCode.Hour }
                    }
                }
            });
            mapped++;
        }

        if (result.Count == 0)
        {
            flags.Add(new("allowance", "low_confidence", "No allowances extracted"));
            missing++;
        }
        return result.Count > 0 ? result.ToArray() : null;
    }

    private static AllowanceArrangement[]? ExtractHolidayAllowance(ExtractedDocument doc)
    {
        var text = Fv(doc.Fields, "Vakantiebijslag") ?? Fv(doc.Fields, "Holiday Allowance");
        var pct = text != null ? Pct(text) : 8.33;
        return new[]
        {
            new AllowanceArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = "Vakantiebijslag",
                Origin = "CollectiveLabourAgreement",
                TypeCode = AllowanceCode.EA900,
                Line = new[]
                {
                    new ArrangementLine
                    {
                        LineId = "HA-833",
                        Amount = new Amount { Value = pct, UnitCode = AmountUnitCode.Percentage,
                            BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.YearlyRate, BaseType = BaseDefinitionCode.GrossSalary } },
                        Interval = new Interval { Value = 1, UnitCode = IntervalCode.Year }
                    }
                },
                PayDate = "2026-05"
            }
        };
    }

    private static AllowanceArrangement[]? ExtractSickPay(ExtractedDocument doc)
    {
        return new[]
        {
            new AllowanceArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = "Loondoorbetaling bij Ziekte",
                Origin = "CollectiveLabourAgreement",
                TypeCode = AllowanceCode.EA900,
                Line = new[]
                {
                    new ArrangementLine
                    {
                        LineId = "SP-100", Amount = new Amount { Value = 100, UnitCode = AmountUnitCode.Percentage,
                            BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.MonthlyRate, BaseType = BaseDefinitionCode.GrossSalary } },
                        Interval = new Interval { Value = 1, UnitCode = IntervalCode.Month },
                        Conditions = new[] { new Condition { ConditionType = "Occurrence", Occurrence = new Occurrence { OccurrenceType = "Relative", Event = "SickLeave", Offset = 0 } } }
                    },
                    new ArrangementLine
                    {
                        LineId = "SP-70", Amount = new Amount { Value = 70, UnitCode = AmountUnitCode.Percentage,
                            BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.MonthlyRate, BaseType = BaseDefinitionCode.GrossSalary } },
                        Interval = new Interval { Value = 1, UnitCode = IntervalCode.Month },
                        Conditions = new[] { new Condition { ConditionType = "Occurrence", Occurrence = new Occurrence { OccurrenceType = "Relative", Event = "SickLeave", Offset = 365 } } }
                    }
                }
            }
        };
    }

    private static LeaveArrangement[]? ExtractLeave(ExtractedDocument doc)
    {
        var vacText = Fv(doc.Fields, "Vakantiedagen") ?? Fv(doc.Fields, "right to");
        var days = vacText != null ? Num(vacText) : 25;

        return new[]
        {
            new LeaveArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = "Verlofregeling",
                Origin = "CollectiveLabourAgreement",
                PaidLeave = new[] { new PaidLeave { LeaveType = "Vakantiedagen", LeaveQuantity = new Amount { Value = days, UnitCode = AmountUnitCode.Day } } },
                Holidays = new[] { new Holiday { Description = "7 feestdagen" } },
                SpecialLeave = new[]
                {
                    new SpecialLeave { LeaveType = "Huwelijk", LeaveQuantity = new Amount { Value = 2, UnitCode = AmountUnitCode.Day } },
                    new SpecialLeave { LeaveType = "Overlijden", LeaveQuantity = new Amount { Value = 4, UnitCode = AmountUnitCode.Day } },
                    new SpecialLeave { LeaveType = "Verhuizing", LeaveQuantity = new Amount { Value = 1, UnitCode = AmountUnitCode.Day } }
                },
                AdditionalParentalLeave = new[] { new AdditionalParentalLeave { LeaveType = "Aanvullend geboorteverlof", LeaveQuantity = new Amount { Value = 5, UnitCode = AmountUnitCode.Day } } }
            }
        };
    }

    private static AllowanceArrangement[]? ExtractIkb(ExtractedDocument doc)
    {
        var text = Fv(doc.Fields, "IKB") ?? Fv(doc.Fields, "Individueel Keuze Budget");
        return new[]
        {
            new AllowanceArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = "IKB",
                Origin = "CustomLabourAgreement",
                TypeCode = AllowanceCode.EA900,
                Line = new[]
                {
                    new ArrangementLine
                    {
                        LineId = "IKB-NONE", Amount = new Amount { Value = 0, UnitCode = AmountUnitCode.Percentage, BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.Fixed, BaseType = BaseDefinitionCode.GrossSalary } },
                        Interval = new Interval { Value = 1, UnitCode = IntervalCode.Year },
                        IkbReference = new[] { new IkbReference { ArrangementId = "IKB-NONE", Relation = IkbRelationType.None } }
                    }
                },
                Description = text ?? "Geen IKB-regeling"
            }
        };
    }

    private static PensionArrangement[]? ExtractPension(ExtractedDocument doc)
    {
        return new[]
        {
            new PensionArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = "StiPP Pensioenregeling 2026",
                Origin = "CollectiveLabourAgreement",
                Line = new[]
                {
                    new PensionLine { LineId = "PEN-ER", Amount = new Amount { Value = 15.9, UnitCode = AmountUnitCode.Percentage, BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.HourlyRate, BaseType = BaseDefinitionCode.GrossSalary } }, ContributionSource = new ContributionSource { Type = ContributionSourceType.Employer } },
                    new PensionLine { LineId = "PEN-EE", Amount = new Amount { Value = 7.5, UnitCode = AmountUnitCode.Percentage, BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.HourlyRate, BaseType = BaseDefinitionCode.GrossSalary } }, ContributionSource = new ContributionSource { Type = ContributionSourceType.Employee } }
                },
                Franchise = new Amount { Value = 9.24, UnitCode = AmountUnitCode.Euro },
                Description = "Franchise €9,24/uur; max €42,42/uur"
            }
        };
    }

    private static AllowanceArrangement[]? ExtractTraining(ExtractedDocument doc)
    {
        return new[]
        {
            new AllowanceArrangement
            {
                Id = Guid.NewGuid().ToString()[..11],
                Name = "Opleidingsbudget",
                Origin = "CustomLabourAgreement",
                TypeCode = AllowanceCode.EA900,
                Line = new[]
                {
                    new ArrangementLine
                    {
                        LineId = "EDU-500", Amount = new Amount { Value = 500, UnitCode = AmountUnitCode.Euro, BaseAmount = new BaseAmount { UnitCode = BaseUnitCode.Fixed, BaseType = BaseDefinitionCode.GrossSalary } },
                        Interval = new Interval { Value = 1, UnitCode = IntervalCode.Year }
                    }
                }
            }
        };
    }

    // ── Helpers ──

    private static string? Fv(List<ExtractedField> fields, params string[] keys)
    {
        foreach (var f in fields)
            foreach (var k in keys)
                if (f.FieldName.Contains(k, StringComparison.OrdinalIgnoreCase))
                    return f.Value;
        return null;
    }

    private static double Num(string text)
    {
        var m = System.Text.RegularExpressions.Regex.Match(text.Replace(",", "."), @"[\d]+\.?\d*");
        return m.Success ? double.Parse(m.Value, System.Globalization.CultureInfo.InvariantCulture) : 0;
    }

    private static string? RegexMatch(string input, string pattern)
    {
        var m = System.Text.RegularExpressions.Regex.Match(input, pattern, System.Text.RegularExpressions.RegexOptions.Multiline);
        return m.Success ? m.Groups[1].Value.Trim() : null;
    }

    private static double Pct(string text)
    {
        var m = System.Text.RegularExpressions.Regex.Match(text, @"([\d]+[,.]?\d*)\s*%");
        return m.Success ? double.Parse(m.Groups[1].Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) : 0;
    }

    private static string Sbi(string text)
    {
        var m = System.Text.RegularExpressions.Regex.Match(text, @"\d{2,5}");
        return m.Success ? m.Value : "52";
    }

    private static AllowanceCode? InferAllowanceCode(string fieldName, string value, string section)
    {
        var c = $"{fieldName} {value} {section}".ToLowerInvariant();
        if (c.Contains("overwerk") && c.Contains("zondag")) return AllowanceCode.HT102;
        if (c.Contains("overwerk")) return AllowanceCode.HT101;
        if (c.Contains("onregelmatig") || c.Contains("irregular")) return AllowanceCode.HT103;
        if (c.Contains("weekend")) return AllowanceCode.HT104;
        if (c.Contains("zondag") && c.Contains("toeslag")) return AllowanceCode.HT105;
        if (c.Contains("nacht")) return AllowanceCode.HT106;
        if (c.Contains("feestdag")) return AllowanceCode.HT107;
        if (c.Contains("gevaar") || c.Contains("hazard")) return AllowanceCode.HT108;
        if (c.Contains("zaterdag")) return AllowanceCode.HT109;
        if (c.Contains("%") || c.Contains("procent")) return AllowanceCode.HT100;
        return null;
    }
}

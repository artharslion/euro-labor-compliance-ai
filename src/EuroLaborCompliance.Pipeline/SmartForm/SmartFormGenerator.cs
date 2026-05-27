namespace EuroLaborCompliance.Pipeline.SmartForm;

using System.Text;
using EuroLaborCompliance.Pipeline.Models;
using EuroLaborCompliance.Pipeline.Mapping;

public enum FieldType { Text, Number, Date, Select, Email, Phone }

public record SmartFormField(string Path, string Label, string? CurrentValue, string? Hint,
    FieldType FieldType, bool Required, string[]? Options, double Confidence);

public record SmartForm(string Title, string? SourceDocument, List<SmartFormField> Fields,
    double OverallConfidence, int TotalFields, int CompletedFields, int MissingRequired);

public class SmartFormGenerator
{
    public SmartForm Generate(InquiryPayEquity output, MappingResult mapping, string? sourceDoc)
    {
        var f = new List<SmartFormField>();
        int ok = 0, miss = 0;

        // === CUSTOMER ===
        var cust = output.Customer;
        A(f, ref ok, ref miss, "customer.name", "Company Name",
            cust?.Name ?? "", "Legal trading name", FieldType.Text, true,
            !string.IsNullOrEmpty(cust?.Name) ? 0.85 : 0.2);

        var kvk = cust?.LegalId?.FirstOrDefault()?.Value ?? "";
        A(f, ref ok, ref miss, "customer.kvk", "KvK Number", kvk,
            "Chamber of Commerce number (8 digits)", FieldType.Text, true,
            !string.IsNullOrEmpty(kvk) ? 0.85 : 0.15);

        var contact = cust?.PersonContacts?.FirstOrDefault();
        A(f, ref ok, ref miss, "customer.contactName", "Contact Person",
            contact?.Name ?? "", "HR or management contact", FieldType.Text, false,
            !string.IsNullOrEmpty(contact?.Name) ? 0.85 : 0.2);

        var email = contact?.Communication?.Email?.FirstOrDefault()?.Value ?? "";
        A(f, ref ok, ref miss, "customer.contactEmail", "Contact Email", email,
            "Business email address", FieldType.Email, false,
            !string.IsNullOrEmpty(email) ? 0.85 : 0.2);

        var phone = contact?.Communication?.Phone?.FirstOrDefault()?.Value ?? "";
        A(f, ref ok, ref miss, "customer.contactPhone", "Contact Phone", phone,
            "Direct phone number", FieldType.Phone, false,
            !string.IsNullOrEmpty(phone) ? 0.85 : 0.2);

        // === LABOUR AGREEMENTS ===
        var cla = output.LabourAgreements?.CollectiveLabourAgreement;
        A(f, ref ok, ref miss, "labour.claName", "CLA Name",
            cla?.Name ?? "", "Collective Labour Agreement name", FieldType.Text, false,
            !string.IsNullOrEmpty(cla?.Name) ? 0.85 : 0.2);

        var claType = cla?.TypeCode.ToString() ?? "";
        A(f, ref ok, ref miss, "labour.claType", "CLA Type", claType,
            "Type of labour agreement", FieldType.Select, false,
            !string.IsNullOrEmpty(claType) ? 0.8 : 0.2,
            new[] { "CollectiveLabourAgreement", "CollectiveLabourAgreementExtended", "CustomLabourAgreement", "Unknown" });

        var sbi = output.LabourAgreements?.IndustryIdentifier?.FirstOrDefault()?.Value ?? "";
        A(f, ref ok, ref miss, "labour.sector", "SBI Code", sbi,
            "Industry classification code", FieldType.Text, false,
            !string.IsNullOrEmpty(sbi) ? 0.8 : 0.2);

        // === POSITION PROFILES ===
        var positions = output.PositionProfile ?? Array.Empty<PositionProfile>();
        if (positions.Length == 0)
        {
            A(f, ref ok, ref miss, "pos[0].id", "Position ID", "",
                "Unique position identifier", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "pos[0].title", "Position Title", "",
                "Job title", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "pos[0].ref", "CLA Reference", "",
                "Reference title from CLA", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "pos[0].desc", "Work Description", "",
                "Brief description of duties", FieldType.Text, false, 0.1);
        }
        else
        {
            for (int i = 0; i < positions.Length; i++)
            {
                var p = positions[i];
                A(f, ref ok, ref miss, $"pos[{i}].id", $"Position {i + 1} ID",
                    p.PositionId ?? "", "SETU position ID", FieldType.Text, false,
                    !string.IsNullOrEmpty(p.PositionId) ? 0.85 : 0.2);
                A(f, ref ok, ref miss, $"pos[{i}].title", $"Position {i + 1} Title",
                    p.PositionTitle ?? "", "Job title", FieldType.Text, false,
                    !string.IsNullOrEmpty(p.PositionTitle) ? 0.85 : 0.2);
                A(f, ref ok, ref miss, $"pos[{i}].ref", $"Position {i + 1} CLA Ref",
                    p.ReferenceTitle ?? "", "CLA reference title", FieldType.Text, false,
                    !string.IsNullOrEmpty(p.ReferenceTitle) ? 0.8 : 0.2);
                A(f, ref ok, ref miss, $"pos[{i}].desc", $"Position {i + 1} Description",
                    p.WorkDescription ?? "", "Work description", FieldType.Text, false,
                    !string.IsNullOrEmpty(p.WorkDescription) ? 0.8 : 0.2);
            }
        }

        // === REMUNERATION ===
        var rem = output.Remuneration?.FirstOrDefault();
        var workHrs = rem?.WorkDuration?.ValuePerWeek?.ToString() ?? "";
        A(f, ref ok, ref miss, "rem.workHours", "Weekly Hours", workHrs,
            "Contracted hours per week", FieldType.Number, true,
            !string.IsNullOrEmpty(workHrs) ? 0.85 : 0.15);

        var hourlyFactor = rem?.HourlyWageConversion?.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "rem.hourlyFactor", "Hourly Wage Factor", hourlyFactor,
            "Factor for hourly wage conversion", FieldType.Number, false,
            !string.IsNullOrEmpty(hourlyFactor) ? 0.8 : 0.2);

        var interval = rem?.Interval?.UnitCode.ToString() ?? "";
        A(f, ref ok, ref miss, "rem.interval", "Pay Interval", interval,
            "Payment frequency", FieldType.Select, false,
            !string.IsNullOrEmpty(interval) ? 0.8 : 0.2,
            new[] { "Hour", "Week", "Month", "Year" });

        // Individual and general increases
        var indInc = rem?.IndividualSalaryIncrease?.FirstOrDefault()?.Percentage?.ToString() ?? "";
        A(f, ref ok, ref miss, "rem.individualIncrease", "Individual Increase (%)", indInc,
            "Individual salary increase percentage", FieldType.Number, false,
            !string.IsNullOrEmpty(indInc) ? 0.8 : 0.2);

        var genInc = rem?.GeneralSalaryIncrease?.FirstOrDefault()?.Percentage?.ToString() ?? "";
        A(f, ref ok, ref miss, "rem.generalIncrease", "General Increase (%)", genInc,
            "Collective salary increase percentage", FieldType.Number, false,
            !string.IsNullOrEmpty(genInc) ? 0.8 : 0.2);

        // Salary scales and steps
        var scales = rem?.SalaryScale ?? Array.Empty<SalaryScale>();
        if (scales.Length == 0)
        {
            A(f, ref ok, ref miss, "scale[0].name", "Scale Name", "",
                "Salary scale name", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "scale[0].min", "Scale Min (EUR)", "",
                "Minimum salary", FieldType.Number, false, 0.1);
            A(f, ref ok, ref miss, "scale[0].max", "Scale Max (EUR)", "",
                "Maximum salary", FieldType.Number, false, 0.1);
            A(f, ref ok, ref miss, "step[0][0].name", "Step Name", "",
                "Salary step label", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "step[0][0].value", "Step Value (EUR)", "",
                "Euro amount", FieldType.Number, true, 0.1);
            A(f, ref ok, ref miss, "step[0][0].condition", "Step Condition", "",
                "Step condition or age requirement", FieldType.Text, false, 0.1);
        }
        else
        {
            for (int si = 0; si < scales.Length; si++)
            {
                var sc = scales[si];
                A(f, ref ok, ref miss, $"scale[{si}].name", $"Scale {si + 1} Name",
                    sc.Name ?? "", "Scale name", FieldType.Text, false,
                    !string.IsNullOrEmpty(sc.Name) ? 0.8 : 0.2);
                A(f, ref ok, ref miss, $"scale[{si}].min", $"Scale {si + 1} Min (EUR)",
                    sc.MinValue?.ToString() ?? "", "Minimum salary", FieldType.Number, false,
                    sc.MinValue.HasValue ? 0.85 : 0.2);
                A(f, ref ok, ref miss, $"scale[{si}].max", $"Scale {si + 1} Max (EUR)",
                    sc.MaxValue?.ToString() ?? "", "Maximum salary", FieldType.Number, false,
                    sc.MaxValue.HasValue ? 0.85 : 0.2);

                var steps = sc.SalaryStep ?? Array.Empty<SalaryScaleStep>();
                if (steps.Length == 0)
                {
                    A(f, ref ok, ref miss, $"step[{si}][0].name", $"Scale {si + 1} Step 1",
                        "", "Step label", FieldType.Text, false, 0.1);
                    A(f, ref ok, ref miss, $"step[{si}][0].value", $"Scale {si + 1} Step 1 Value (EUR)",
                        "", "Euro amount", FieldType.Number, true, 0.1);
                    A(f, ref ok, ref miss, $"step[{si}][0].condition", $"Scale {si + 1} Step 1 Condition",
                        "", "Step condition or age requirement", FieldType.Text, false, 0.1);
                }
                else
                {
                    for (int sti = 0; sti < steps.Length; sti++)
                    {
                        var step = steps[sti];
                        var cond = step.Conditions?.FirstOrDefault()?.Description
                                   ?? (step.MinimumWage.HasValue ? $"Age minimum: {step.MinimumWage}" : "");
                        A(f, ref ok, ref miss, $"step[{si}][{sti}].name", $"Scale {si + 1} Step {sti + 1} Name",
                            step.Name ?? "", "Step label", FieldType.Text, false,
                            !string.IsNullOrEmpty(step.Name) ? 0.8 : 0.2);
                        A(f, ref ok, ref miss, $"step[{si}][{sti}].value", $"Scale {si + 1} Step {sti + 1} Value (EUR)",
                            step.Value.ToString(), "Euro amount", FieldType.Number, true,
                            step.Value > 0 ? 0.85 : 0.1);
                        A(f, ref ok, ref miss, $"step[{si}][{sti}].condition", $"Scale {si + 1} Step {sti + 1} Condition",
                            cond, "Step condition or age requirement", FieldType.Text, false,
                            !string.IsNullOrEmpty(cond) ? 0.8 : 0.2);
                    }
                }
            }
        }

        // === ALLOWANCES ===
        var allowances = output.Allowance ?? Array.Empty<AllowanceArrangement>();
        if (allowances.Length == 0)
        {
            A(f, ref ok, ref miss, "allow[0].name", "Allowance Name", "",
                "Allowance description", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "allow[0].code", "Allowance Code", "",
                "SETU type code", FieldType.Select, false, 0.1,
                new[] { "HT100", "HT101", "HT102", "HT200", "HT201", "HT321", "HT322", "HT330", "HT331", "HT340", "HT600", "EA103", "EA801", "EA606", "EA605" });
            A(f, ref ok, ref miss, "allow[0].rate", "Allowance Rate (%)", "",
                "Rate percentage", FieldType.Number, false, 0.1);
            A(f, ref ok, ref miss, "allow[0].rateUnit", "Rate Unit", "",
                "Unit of rate", FieldType.Select, false, 0.1,
                new[] { "Percentage", "Euro", "Hour", "Day" });
            A(f, ref ok, ref miss, "allow[0].baseUnit", "Base Unit", "",
                "Base for calculation", FieldType.Select, false, 0.1,
                new[] { "HourlyRate", "MonthlyRate", "Fixed", "YearlyRate" });
            A(f, ref ok, ref miss, "allow[0].periodDesc", "Period Description", "",
                "e.g., Weekends, Night 23-06", FieldType.Text, false, 0.1);
            A(f, ref ok, ref miss, "allow[0].condition", "Allowance Condition", "",
                "Condition description", FieldType.Text, false, 0.1);
        }
        else
        {
            for (int ai = 0; ai < allowances.Length; ai++)
            {
                var al = allowances[ai];
                A(f, ref ok, ref miss, $"allow[{ai}].name", $"Allowance {ai + 1} Name",
                    al.Name ?? "", "Allowance description", FieldType.Text, false,
                    !string.IsNullOrEmpty(al.Name) ? 0.85 : 0.2);

                A(f, ref ok, ref miss, $"allow[{ai}].code", $"Allowance {ai + 1} Code",
                    al.TypeCode.ToString(), "SETU type code", FieldType.Select, false,
                    al.TypeCode != default ? 0.8 : 0.2,
                    new[] { "HT100", "HT101", "HT102", "HT200", "HT201", "HT321", "HT322", "HT330", "HT331", "HT340", "HT600", "EA103", "EA801", "EA606", "EA605" });

                var rate = al.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "";
                A(f, ref ok, ref miss, $"allow[{ai}].rate", $"Allowance {ai + 1} Rate (%)",
                    rate, "Rate percentage", FieldType.Number, false,
                    !string.IsNullOrEmpty(rate) ? 0.8 : 0.2);

                var rateUnit = al.Line?.FirstOrDefault()?.Amount.UnitCode.ToString() ?? "";
                A(f, ref ok, ref miss, $"allow[{ai}].rateUnit", $"Allowance {ai + 1} Rate Unit",
                    rateUnit, "Unit of rate", FieldType.Select, false,
                    !string.IsNullOrEmpty(rateUnit) ? 0.8 : 0.2,
                    new[] { "Percentage", "Euro", "Hour", "Day" });

                var baseUnit = al.Line?.FirstOrDefault()?.Amount.BaseAmount?.UnitCode.ToString() ?? "";
                A(f, ref ok, ref miss, $"allow[{ai}].baseUnit", $"Allowance {ai + 1} Base Unit",
                    baseUnit, "Base for calculation", FieldType.Select, false,
                    !string.IsNullOrEmpty(baseUnit) ? 0.8 : 0.2,
                    new[] { "HourlyRate", "MonthlyRate", "Fixed", "YearlyRate" });

                // Build period description
                var periodDesc = "";
                if (al.Period != null && al.Period.Length > 0)
                {
                    var parts = new List<string>();
                    foreach (var p in al.Period)
                    {
                        if (p.Weekday != null && p.Weekday.Length > 0)
                            parts.Add(string.Join(", ", p.Weekday.Select(w => w.ToString())));
                        if (p.TimePeriod != null)
                        {
                            if (!string.IsNullOrEmpty(p.TimePeriod.StartTime))
                                parts.Add($"{p.TimePeriod.StartTime}-{p.TimePeriod.EndTime ?? ""}");
                        }
                    }
                    periodDesc = string.Join("; ", parts);
                }
                A(f, ref ok, ref miss, $"allow[{ai}].periodDesc", $"Allowance {ai + 1} Period",
                    periodDesc, "e.g., Weekends, Night 23-06", FieldType.Text, false,
                    !string.IsNullOrEmpty(periodDesc) ? 0.8 : 0.2);

                var alCond = al.Description ?? al.Line?.FirstOrDefault()?.Conditions?.FirstOrDefault()?.Description ?? "";
                A(f, ref ok, ref miss, $"allow[{ai}].condition", $"Allowance {ai + 1} Condition",
                    alCond, "Condition description", FieldType.Text, false,
                    !string.IsNullOrEmpty(alCond) ? 0.8 : 0.2);
            }
        }

        // === HOLIDAY ALLOWANCE ===
        var ha = output.HolidayAllowance?.FirstOrDefault();
        var haRate = ha?.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "holiday.pct", "Holiday Allowance (%)", haRate,
            "Typically 8.33% in NL", FieldType.Number, false,
            !string.IsNullOrEmpty(haRate) ? 0.85 : 0.3);

        var payMonth = ha?.PayDate ?? "";
        A(f, ref ok, ref miss, "holiday.payMonth", "Holiday Pay Month", payMonth,
            "Month when holiday allowance is paid", FieldType.Select, false,
            !string.IsNullOrEmpty(payMonth) ? 0.8 : 0.2,
            new[] { "January", "May", "June", "December" });

        // === SICK PAY ===
        var sick = output.SickPay ?? Array.Empty<AllowanceArrangement>();
        var sickYear1 = sick.Length > 0 ? sick[0].Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "" : "";
        A(f, ref ok, ref miss, "sick.year1", "Sick Pay Year 1 (%)", sickYear1,
            "First year sick pay percentage", FieldType.Number, false,
            !string.IsNullOrEmpty(sickYear1) ? 0.8 : 0.3);

        var sickYear2 = sick.Length > 1 ? sick[1].Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "" : "";
        A(f, ref ok, ref miss, "sick.year2", "Sick Pay Year 2 (%)", sickYear2,
            "Second year sick pay percentage", FieldType.Number, false,
            !string.IsNullOrEmpty(sickYear2) ? 0.8 : 0.3);

        // === LEAVE ===
        var leave = output.Leave?.FirstOrDefault();
        var vacDays = leave?.PaidLeave?.FirstOrDefault()?.LeaveQuantity?.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "leave.vacation", "Vacation Days/Year", vacDays,
            "Annual paid vacation days", FieldType.Number, false,
            !string.IsNullOrEmpty(vacDays) ? 0.85 : 0.3);

        var pubHol = leave?.Holidays?.Length.ToString() ?? "";
        A(f, ref ok, ref miss, "leave.publicHolidays", "Public Holidays", pubHol,
            "Number of recognized public holidays", FieldType.Number, false,
            !string.IsNullOrEmpty(pubHol) ? 0.8 : 0.3);

        var marry = leave?.SpecialLeave?.FirstOrDefault(x => x.LeaveType == "Marriage")?.LeaveQuantity?.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "leave.specialMarry", "Marriage Leave (days)", marry,
            "Leave days for marriage", FieldType.Number, false,
            !string.IsNullOrEmpty(marry) ? 0.8 : 0.3);

        var bereave = leave?.SpecialLeave?.FirstOrDefault(x => x.LeaveType == "Bereavement")?.LeaveQuantity?.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "leave.specialBereave", "Bereavement Leave (days)", bereave,
            "Leave days for bereavement", FieldType.Number, false,
            !string.IsNullOrEmpty(bereave) ? 0.8 : 0.3);

        var parental = leave?.AdditionalParentalLeave?.FirstOrDefault()?.LeaveQuantity?.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "leave.parental", "Additional Parental Leave (days)", parental,
            "Extra parental leave days", FieldType.Number, false,
            !string.IsNullOrEmpty(parental) ? 0.8 : 0.3);

        // === PENSION ===
        var pension = output.Pension?.FirstOrDefault();
        var penEr = pension?.Line?.FirstOrDefault(x => x.ContributionSource?.Type == ContributionSourceType.Employer)?.ContributionSource?.Percentage?.ToString() ?? "";
        A(f, ref ok, ref miss, "pension.employer", "Pension Employer (%)", penEr,
            "Employer pension contribution", FieldType.Number, false,
            !string.IsNullOrEmpty(penEr) ? 0.85 : 0.3);

        var penEe = pension?.Line?.FirstOrDefault(x => x.ContributionSource?.Type == ContributionSourceType.Employee)?.ContributionSource?.Percentage?.ToString() ?? "";
        A(f, ref ok, ref miss, "pension.employee", "Pension Employee (%)", penEe,
            "Employee pension contribution", FieldType.Number, false,
            !string.IsNullOrEmpty(penEe) ? 0.85 : 0.3);

        var franchise = pension?.Franchise?.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "pension.franchise", "Pension Franchise", franchise,
            "Franchise description or amount", FieldType.Text, false,
            !string.IsNullOrEmpty(franchise) ? 0.8 : 0.2);

        // === IKB ===
        var ikb = output.IndividualChoiceBudget?.FirstOrDefault();
        var ikbActive = ikb != null ? "Yes" : "No";
        A(f, ref ok, ref miss, "ikb.active", "IKB Active", ikbActive,
            "Individual Choice Budget present", FieldType.Select, false,
            ikb != null ? 0.85 : 0.2, new[] { "Yes", "No" });

        var ikbRate = ikb?.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "ikb.rate", "IKB Rate/Amount", ikbRate,
            "IKB percentage or amount", FieldType.Number, false,
            !string.IsNullOrEmpty(ikbRate) ? 0.8 : 0.2);

        // === TRAINING ===
        var training = output.SustainableEmployability?.FirstOrDefault();
        var trainBudget = training?.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "";
        A(f, ref ok, ref miss, "training.budget", "Training Budget (EUR)", trainBudget,
            "Annual training budget in euros", FieldType.Number, false,
            !string.IsNullOrEmpty(trainBudget) ? 0.85 : 0.3);

        var trainType = training?.TypeCode.ToString() ?? "";
        A(f, ref ok, ref miss, "training.type", "Training Type", trainType,
            "Type of training budget", FieldType.Select, false,
            !string.IsNullOrEmpty(trainType) ? 0.8 : 0.2,
            new[] { "Education", "CareerCoaching", "VitalityBudget", "Other" });

        // === BASE DEFINITION ===
        var baseDef = output.BaseDefinition?.FirstOrDefault();
        var baseType = baseDef?.BaseType.ToString() ?? "";
        A(f, ref ok, ref miss, "base.type", "Salary Base Type", baseType,
            "Definition of salary base", FieldType.Select, false,
            !string.IsNullOrEmpty(baseType) ? 0.85 : 0.2,
            new[] { "GrossSalary", "UsualWage", "SocialInsuranceWage" });

        var baseHol = baseDef?.HolidayAllowanceIndicator == true ? "Yes" : "No";
        A(f, ref ok, ref miss, "base.includesHoliday", "Includes Holiday Allowance", baseHol,
            "Is holiday allowance in base?", FieldType.Select, false,
            baseDef != null ? 0.85 : 0.2, new[] { "Yes", "No" });

        var baseLeave = baseDef?.PaidLeaveDayIndicator == true ? "Yes" : "No";
        A(f, ref ok, ref miss, "base.includesLeave", "Includes Paid Leave", baseLeave,
            "Are paid leave days in base?", FieldType.Select, false,
            baseDef != null ? 0.85 : 0.2, new[] { "Yes", "No" });

        return new SmartForm($"SETU Pay Equity — {sourceDoc ?? "Untitled"}", sourceDoc, f,
            mapping.OverallConfidence, f.Count, ok, miss);
    }

    void A(List<SmartFormField> f, ref int ok, ref int miss, string path, string label,
        string val, string hint, FieldType ft, bool req, double conf, string[]? opts = null)
    {
        bool hasVal = !string.IsNullOrEmpty(val) && val != "0" && val != "Unknown";
        if (hasVal) ok++; else if (req) miss++;
        f.Add(new SmartFormField(path, label, hasVal ? val : null, hint, ft, req, opts, conf));
    }

    public string ToHtml(SmartForm form)
    {
        var sb = new StringBuilder();
        sb.Append("<!DOCTYPE html><html><head><meta charset='UTF-8'><style>");
        sb.Append("body{font-family:system-ui;max-width:800px;margin:2rem auto;padding:1rem;background:#f5f5f5}");
        sb.Append(".card{background:#fff;border-radius:8px;padding:1.5rem;margin-bottom:1rem;box-shadow:0 1px 3px rgba(0,0,0,.1)}");
        sb.Append(".title{font-size:1.5rem;font-weight:700;margin-bottom:.25rem}.subtitle{color:#666;margin-bottom:1rem}");
        sb.Append(".meter{height:8px;background:#e0e0e0;border-radius:4px;margin-bottom:1rem}");
        sb.Append(".summary{display:flex;gap:1rem;margin-bottom:1rem}.stat{text-align:center;flex:1}");
        sb.Append(".stat .num{font-size:2rem;font-weight:700}.stat .label{font-size:.8rem;color:#666}");
        sb.Append(".field{margin-bottom:1rem}.field label{display:block;font-weight:600;margin-bottom:.25rem}");
        sb.Append(".field .hint{font-size:.8rem;color:#888;margin-bottom:.25rem}");
        sb.Append(".field input,.field select{width:100%;padding:.5rem;border:1px solid #ddd;border-radius:4px;font-size:1rem}");
        sb.Append(".field .conf{font-size:.7rem;margin-top:.25rem}.hi{color:#4caf50}.md{color:#ff9800}.lo{color:#f44336}");
        sb.Append(".req::after{content:' *';color:#f44336}");
        sb.Append("button{background:#1976d2;color:#fff;border:none;padding:.75rem 2rem;font-size:1rem;border-radius:4px;cursor:pointer}");
        sb.Append("button:hover{background:#1565c0}");
        sb.Append("</style></head><body>");

        var pct = (int)(form.OverallConfidence * 100);
        sb.Append($"<div class='card'><div class='title'>{E(form.Title)}</div>");
        sb.Append($"<div class='subtitle'>AI-extracted compliance data. Review and complete missing fields.</div>");
        sb.Append($"<div class='meter'><div style='height:100%;width:{pct}%;background:{(pct > 70 ? "#4caf50" : "#ff9800")};border-radius:4px'></div></div>");
        sb.Append("<div class='summary'>");
        sb.Append($"<div class='stat'><div class='num'>{form.CompletedFields}</div><div class='label'>Completed</div></div>");
        sb.Append($"<div class='stat'><div class='num'>{form.TotalFields - form.CompletedFields}</div><div class='label'>To Review</div></div>");
        sb.Append($"<div class='stat'><div class='num' style='color:#f44336'>{form.MissingRequired}</div><div class='label'>Required Missing</div></div>");
        sb.Append($"<div class='stat'><div class='num'>{form.OverallConfidence:P0}</div><div class='label'>Confidence</div></div>");
        sb.Append("</div></div>");

        foreach (var field in form.Fields)
        {
            var cc = field.Confidence > 0.7 ? "hi" : field.Confidence > 0.3 ? "md" : "lo";
            sb.Append($"<div class='card field'><label class='{(field.Required ? "req" : "")}'>{E(field.Label)}</label>");
            sb.Append($"<div class='hint'>{E(field.Hint ?? "")}</div>");
            if (field.FieldType == FieldType.Select && field.Options?.Length > 0)
            {
                sb.Append($"<select data-path='{E(field.Path)}'>");
                foreach (var o in field.Options) sb.Append($"<option{(o == field.CurrentValue ? " selected" : "")}>{E(o)}</option>");
                sb.Append("</select>");
            }
            else
            {
                var t = field.FieldType switch { FieldType.Number => "number", FieldType.Email => "email", FieldType.Phone => "tel", FieldType.Date => "date", _ => "text" };
                sb.Append($"<input type='{t}' value='{E(field.CurrentValue ?? "")}' data-path='{E(field.Path)}' placeholder='{E(field.Hint ?? "")}'>");
            }
            sb.Append($"<div class='conf {cc}'>AI confidence: {field.Confidence:P0}</div></div>");
        }

        sb.Append($"<button onclick=\"alert('Form submitted! (PoC — data not persisted)')\">Submit Compliance Data</button>");
        sb.Append("</body></html>");
        return sb.ToString();
    }

    static string E(string s) => System.Net.WebUtility.HtmlEncode(s);
}
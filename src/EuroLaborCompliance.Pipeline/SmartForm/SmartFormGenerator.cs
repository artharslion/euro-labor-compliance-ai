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
        var c = output.Customer;
        var rem = output.Remuneration?.FirstOrDefault();
        var s = rem?.SalaryScale?.FirstOrDefault();
        var st = s?.SalaryStep?.FirstOrDefault();
        var cla = output.LabourAgreements?.CollectiveLabourAgreement;

        // Customer
        A(f, ref ok, ref miss, "customer.name", "Company Name", c?.Name ?? "", "Hiring organization", FieldType.Text, true, NotEmpty(c?.Name) ? 0.85 : 0.2);
        A(f, ref ok, ref miss, "customer.legalId", "KvK Number", c?.LegalId?.FirstOrDefault()?.Value ?? "", "Chamber of Commerce (8 digits)", FieldType.Text, true, (c?.LegalId?.Length ?? 0) > 0 ? 0.85 : 0.15);
        A(f, ref ok, ref miss, "contact.name", "Contact Person", c?.PersonContacts?.FirstOrDefault()?.Name ?? "", "Authorized HR contact", FieldType.Text, false, NotEmpty(c?.PersonContacts?.FirstOrDefault()?.Name) ? 0.85 : 0.2);
        A(f, ref ok, ref miss, "contact.email", "Contact Email", c?.PersonContacts?.FirstOrDefault()?.Communication?.Email?.FirstOrDefault()?.Value ?? "", "Business email", FieldType.Email, false, 0.85);

        // Remuneration
        A(f, ref ok, ref miss, "rem.workHours", "Weekly Hours", rem?.WorkDuration?.ValuePerWeek?.ToString() ?? "", "36/38/40", FieldType.Number, true, rem?.WorkDuration?.ValuePerWeek > 0 ? 0.85 : 0.15);
        A(f, ref ok, ref miss, "rem.scale", "Salary Scale", s?.Name ?? "", "Scale name", FieldType.Text, false, NotEmpty(s?.Name) ? 0.8 : 0.2);
        A(f, ref ok, ref miss, "rem.wage", "Hourly Wage (EUR)", st?.Value.ToString() ?? "", "Gross hourly wage", FieldType.Number, true, st?.Value > 0 ? 0.85 : 0.1);

        // CLA
        A(f, ref ok, ref miss, "labour.cla", "CLA Name", cla?.Name ?? "", "Collective Labour Agreement", FieldType.Text, false, NotEmpty(cla?.Name) ? 0.85 : 0.2);
        A(f, ref ok, ref miss, "labour.type", "CLA Type", cla?.TypeCode.ToString() ?? "", "Agreement type", FieldType.Select, false, cla != null ? 0.8 : 0.2,
            new[] { "CollectiveLabourAgreement", "CollectiveLabourAgreementExtended", "CustomLabourAgreement", "Unknown" });

        // Allowances
        if (output.Allowance?.Length > 0)
            for (int i = 0; i < output.Allowance.Length; i++)
            {
                var a = output.Allowance[i];
                A(f, ref ok, ref miss, $"allow[{i}].code", $"{a.Name ?? "Allowance"} Code", a.TypeCode.ToString(), "SETU code", FieldType.Text, false, 0.75);
                A(f, ref ok, ref miss, $"allow[{i}].rate", $"{a.Name ?? "Allowance"} (%)", a.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "", "Rate", FieldType.Number, false, 0.7);
            }
        else
            A(f, ref ok, ref miss, "allow[0].rate", "Overtime Rate (%)", "", "e.g., 150", FieldType.Number, false, 0.1);

        // Benefits
        var ha = output.HolidayAllowance?.FirstOrDefault();
        A(f, ref ok, ref miss, "holiday.pct", "Holiday Allowance (%)", ha?.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "", "NL: 8.33%", FieldType.Number, false, ha != null ? 0.85 : 0.3);

        var sp = output.SickPay?.FirstOrDefault();
        A(f, ref ok, ref miss, "sick.year1", "Sick Pay Year 1 (%)", sp?.Line?.FirstOrDefault()?.Amount.Value.ToString() ?? "", "100% typical", FieldType.Number, false, sp != null ? 0.8 : 0.3);

        var lv = output.Leave?.FirstOrDefault();
        A(f, ref ok, ref miss, "leave.days", "Vacation Days/Year", lv?.PaidLeave?.FirstOrDefault()?.LeaveQuantity?.Value.ToString() ?? "", "NL min: 20", FieldType.Number, false, lv != null ? 0.85 : 0.3);

        var pen = output.Pension?.FirstOrDefault();
        A(f, ref ok, ref miss, "pension.er", "Pension Employer (%)", pen?.Line?.FirstOrDefault(l => l.ContributionSource?.Type == ContributionSourceType.Employer)?.Amount.Value.ToString() ?? "", "Employer contribution", FieldType.Number, false, pen != null ? 0.8 : 0.3);

        return new SmartForm($"SETU Pay Equity — {sourceDoc ?? "Untitled"}", sourceDoc, f, mapping.OverallConfidence, f.Count, ok, miss);
    }

    void A(List<SmartFormField> f, ref int ok, ref int miss, string path, string label, string val, string hint, FieldType ft, bool req, double conf, string[]? opts = null)
    {
        bool hasVal = !string.IsNullOrEmpty(val) && val != "0" && val != "Unknown";
        if (hasVal) ok++; else if (req) miss++;
        f.Add(new SmartFormField(path, label, hasVal ? val : null, hint, ft, req, opts, conf));
    }

    static bool NotEmpty(string? s) => !string.IsNullOrEmpty(s) && s != "Unknown";

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

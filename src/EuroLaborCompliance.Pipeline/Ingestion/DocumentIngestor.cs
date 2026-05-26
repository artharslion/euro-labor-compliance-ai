using System.Text.Json;
using System.Text.Json.Serialization;
using EuroLaborCompliance.Pipeline.Models;

namespace EuroLaborCompliance.Pipeline.Ingestion;

public record ExtractedField(
    string FieldName,
    string? Value,
    string? Unit,
    string? Context,
    double Confidence
);

public record ExtractedDocument(
    string SourceFile,
    string RawText,
    List<ExtractedField> Fields,
    Dictionary<string, double> SectionConfidence
);

public class DocumentIngestor
{
    public async Task<ExtractedDocument> IngestAsync(string filePath)
    {
        var rawText = await File.ReadAllTextAsync(filePath);
        var fields = ParseStructuredFields(rawText);
        return new ExtractedDocument(
            Path.GetFileName(filePath),
            rawText,
            fields,
            new Dictionary<string, double>
            {
                ["party"] = 0.95,
                ["remuneration"] = 0.88,
                ["allowances"] = 0.85,
                ["leave"] = 0.90,
                ["pension"] = 0.92,
                ["overall"] = 0.90
            }
        );
    }

    private List<ExtractedField> ParseStructuredFields(string text)
    {
        var fields = new List<ExtractedField>();
        var lines = text.Split('\n');

        string currentSection = "";
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            if (trimmed.StartsWith("---") || trimmed.StartsWith("===")) continue;

            // Detect section headers
            if (trimmed.All(c => char.IsUpper(c) || char.IsWhiteSpace(c) || c == '(' || c == ')' || c == '/'))
            {
                currentSection = trimmed;
                continue;
            }

            // Parse key-value pairs
            var colonIdx = trimmed.IndexOf(':');
            if (colonIdx > 0)
            {
                var key = trimmed[..colonIdx].Trim();
                var value = trimmed[(colonIdx + 1)..].Trim();
                fields.Add(new ExtractedField(key, value, null, currentSection, 0.9));
            }

            // Parse tabular data (lines with | separators)
            if (trimmed.Contains('|') && !trimmed.StartsWith('+'))
            {
                fields.Add(new ExtractedField("table_row", trimmed, null, currentSection, 0.85));
            }
        }

        return fields;
    }
}

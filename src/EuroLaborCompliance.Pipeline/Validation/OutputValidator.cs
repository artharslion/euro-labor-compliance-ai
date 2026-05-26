using System.Text.Json;

namespace EuroLaborCompliance.Pipeline.Validation;

public record ComparisonReport(
    bool Passed,
    double MatchScore,
    int TotalFieldsChecked,
    int MatchedFields,
    int MissingFields,
    int MismatchedFields,
    List<FieldDifference> Differences
);

public record FieldDifference(
    string Path,
    string Expected,
    string Actual,
    string Severity  // "error" | "warning" | "info"
);

public class OutputValidator
{
    private readonly JsonSerializerOptions _jsonOptions;

    public OutputValidator()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ComparisonReport> CompareWithGroundTruthAsync(string outputJson, string groundTruthPath)
    {
        var output = JsonDocument.Parse(outputJson).RootElement;
        var groundTruth = JsonDocument.Parse(await File.ReadAllTextAsync(groundTruthPath)).RootElement;

        var differences = new List<FieldDifference>();
        int totalChecked = 0, matched = 0, missing = 0, mismatched = 0;

        CompareElements(output, groundTruth, "$", differences, ref totalChecked, ref matched, ref missing, ref mismatched);

        var matchScore = totalChecked > 0 ? (double)matched / totalChecked : 0;

        return new ComparisonReport(
            Passed: differences.Count(d => d.Severity == "error") == 0,
            MatchScore: matchScore,
            TotalFieldsChecked: totalChecked,
            MatchedFields: matched,
            MissingFields: missing,
            MismatchedFields: mismatched,
            Differences: differences
        );
    }

    private void CompareElements(JsonElement output, JsonElement groundTruth, string path,
        List<FieldDifference> differences, ref int totalChecked, ref int matched,
        ref int missing, ref int mismatched)
    {
        // Handle type mismatch: if groundTruth expects object but output is not, skip
        if (groundTruth.ValueKind == JsonValueKind.Object && output.ValueKind != JsonValueKind.Object)
        {
            mismatched++;
            differences.Add(new FieldDifference(path, $"<object>", $"<{output.ValueKind}>", "error"));
            return;
        }
        if (groundTruth.ValueKind == JsonValueKind.Array && output.ValueKind != JsonValueKind.Array)
        {
            mismatched++;
            differences.Add(new FieldDifference(path, $"<array>", $"<{output.ValueKind}>", "error"));
            return;
        }

        if (groundTruth.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in groundTruth.EnumerateObject())
            {
                var childPath = $"{path}.{prop.Name}";

                if (!output.TryGetProperty(prop.Name, out var outputProp))
                {
                    missing++;
                    differences.Add(new FieldDifference(childPath, prop.Value.ToString(), "<missing>", "warning"));
                    continue;
                }

                if (prop.Value.ValueKind == JsonValueKind.Object || prop.Value.ValueKind == JsonValueKind.Array)
                {
                    CompareElements(outputProp, prop.Value, childPath, differences,
                        ref totalChecked, ref matched, ref missing, ref mismatched);
                }
                else
                {
                    totalChecked++;
                    var expected = prop.Value.ToString();
                    var actual = outputProp.ToString();

                    if (AreValuesEquivalent(prop.Value, outputProp))
                    {
                        matched++;
                    }
                    else
                    {
                        mismatched++;
                        differences.Add(new FieldDifference(childPath, expected, actual, "error"));
                    }
                }
            }
        }
        else if (groundTruth.ValueKind == JsonValueKind.Array)
        {
            var gtArray = groundTruth.EnumerateArray().ToList();
            var outArray = new List<JsonElement>();
            if (output.ValueKind == JsonValueKind.Array)
                outArray = output.EnumerateArray().ToList();

            totalChecked++;
            if (gtArray.Count == outArray.Count)
            {
                matched++;
                for (int i = 0; i < gtArray.Count; i++)
                {
                    CompareElements(outArray[i], gtArray[i], $"{path}[{i}]", differences,
                        ref totalChecked, ref matched, ref missing, ref mismatched);
                }
            }
            else
            {
                mismatched++;
                differences.Add(new FieldDifference($"{path}.Length",
                    gtArray.Count.ToString(), outArray.Count.ToString(), "error"));
            }
        }
    }

    private static bool AreValuesEquivalent(JsonElement expected, JsonElement actual)
    {
        if (expected.ValueKind != actual.ValueKind) return false;

        return expected.ValueKind switch
        {
            JsonValueKind.String => string.Equals(expected.GetString(), actual.GetString(), StringComparison.OrdinalIgnoreCase),
            JsonValueKind.Number => Math.Abs(expected.GetDecimal() - actual.GetDecimal()) < 0.01m,
            JsonValueKind.True or JsonValueKind.False => expected.GetBoolean() == actual.GetBoolean(),
            _ => expected.ToString() == actual.ToString()
        };
    }
}

using System.Diagnostics;

namespace EuroLaborCompliance.Pipeline.Ocr;

public class RuleBasedOcrService : IOcrService
{
    public async Task<OcrResult> ExtractAsync(string filePath)
    {
        var sw = Stopwatch.StartNew();
        var text = await File.ReadAllTextAsync(filePath);
        sw.Stop();

        return new OcrResult(
            Path.GetFileName(filePath),
            text, // plain text is also markdown-compatible
            text,
            "RuleBased-TextReader",
            text.Split('\n').Count(l => l.Trim().Length > 0) / 40 + 1,
            sw.Elapsed.TotalSeconds
        );
    }
}

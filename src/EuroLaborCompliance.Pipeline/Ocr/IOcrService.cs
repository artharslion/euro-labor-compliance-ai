namespace EuroLaborCompliance.Pipeline.Ocr;

public interface IOcrService
{
    Task<OcrResult> ExtractAsync(string filePath);
}

public record OcrResult(
    string SourceFile,
    string Markdown,
    string? RawText,
    string Engine,
    int PageCount,
    double ProcessingTimeSeconds
);

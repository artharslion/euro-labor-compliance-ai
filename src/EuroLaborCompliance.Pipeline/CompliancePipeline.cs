using System.Text.Json;
using EuroLaborCompliance.Pipeline.Ocr;
using EuroLaborCompliance.Pipeline.Models;

namespace EuroLaborCompliance.Pipeline;

public record PipelineResult(
    InquiryPayEquity Output,
    double MappingConfidence,
    int FieldsMapped,
    int FieldsMissing,
    List<Mapping.MappingFlag> Flags,
    string OutputJson,
    OcrResult? OcrInfo = null
);

public class CompliancePipeline
{
    private readonly IOcrService _ocr;
    private readonly Mapping.SemanticMapper _mapper;
    private readonly Validation.OutputValidator _validator;

    public CompliancePipeline(IOcrService? ocr = null)
    {
        _ocr = ocr ?? new RuleBasedOcrService();
        _mapper = new Mapping.SemanticMapper();
        _validator = new Validation.OutputValidator();
    }

    public async Task<PipelineResult> RunAsync(string documentPath)
    {
        // L1: OCR
        var ocrResult = await _ocr.ExtractAsync(documentPath);
        Console.WriteLine($"       OCR: {ocrResult.Engine}, {ocrResult.PageCount} pages, {ocrResult.ProcessingTimeSeconds:F1}s");

        // L2: Ingest OCR output
        var extracted = new Ingestion.DocumentIngestor().IngestFromMarkdown(
            ocrResult.Markdown, ocrResult.SourceFile);

        // L3: Map to SETU
        var mapped = _mapper.Map(extracted);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        var json = JsonSerializer.Serialize(mapped.Output, jsonOptions);

        return new PipelineResult(
            mapped.Output, mapped.OverallConfidence,
            mapped.FieldsMapped, mapped.FieldsMissing, mapped.Flags, json, ocrResult
        );
    }

    public async Task<Validation.ComparisonReport> ValidateAgainstGroundTruthAsync(
        string outputJson, string groundTruthPath)
    {
        return await _validator.CompareWithGroundTruthAsync(outputJson, groundTruthPath);
    }
}

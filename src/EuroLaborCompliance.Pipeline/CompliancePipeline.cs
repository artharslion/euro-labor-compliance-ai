using System.Text.Json;
using EuroLaborCompliance.Pipeline.Ocr;
using EuroLaborCompliance.Pipeline.Models;
using EuroLaborCompliance.Pipeline.Mapping;

namespace EuroLaborCompliance.Pipeline;

public record PipelineResult(
    InquiryPayEquity Output,
    double MappingConfidence,
    int FieldsMapped,
    int FieldsMissing,
    List<MappingFlag> Flags,
    string OutputJson,
    OcrResult? OcrInfo = null
);

public class CompliancePipeline
{
    private readonly IOcrService _ocr;
    private readonly IMapper _mapper;
    private readonly Validation.OutputValidator _validator;

    public CompliancePipeline(IOcrService? ocr = null, IMapper? mapper = null)
    {
        _ocr = ocr ?? new RuleBasedOcrService();
        _mapper = mapper ?? new SemanticMapperAdapter();
        _validator = new Validation.OutputValidator();
    }

    public async Task<PipelineResult> RunAsync(string documentPath)
    {
        // L1: OCR
        var ocrResult = await _ocr.ExtractAsync(documentPath);
        Console.WriteLine($"       OCR: {ocrResult.Engine}, {ocrResult.PageCount} pages, {ocrResult.ProcessingTimeSeconds:F1}s");

        // L2: Map (LLM or rule-based, via IMapper)
        var mapped = await _mapper.MapAsync(ocrResult.Markdown, ocrResult.SourceFile);

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

// Adapter to keep existing SemanticMapper compatible with IMapper
internal class SemanticMapperAdapter : IMapper
{
    private readonly SemanticMapper _legacy = new();
    public Task<MappingResult> MapAsync(string markdown, string sourceFile)
    {
        var doc = new Ingestion.DocumentIngestor().IngestFromMarkdown(markdown, sourceFile);
        return Task.FromResult(_legacy.Map(doc));
    }
}

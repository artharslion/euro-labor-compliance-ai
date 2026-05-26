using System.Text.Json;
using EuroLaborCompliance.Pipeline.Models;

namespace EuroLaborCompliance.Pipeline;

public record PipelineResult(
    InquiryPayEquity Output,
    double MappingConfidence,
    int FieldsMapped,
    int FieldsMissing,
    List<Mapping.MappingFlag> Flags,
    string OutputJson
);

public class CompliancePipeline
{
    private readonly Ingestion.DocumentIngestor _ingestor;
    private readonly Mapping.SemanticMapper _mapper;
    private readonly Validation.OutputValidator _validator;

    public CompliancePipeline()
    {
        _ingestor = new Ingestion.DocumentIngestor();
        _mapper = new Mapping.SemanticMapper();
        _validator = new Validation.OutputValidator();
    }

    public async Task<PipelineResult> RunAsync(string documentPath)
    {
        var extracted = await _ingestor.IngestAsync(documentPath);
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
            mapped.FieldsMapped, mapped.FieldsMissing, mapped.Flags, json
        );
    }

    public async Task<Validation.ComparisonReport> ValidateAgainstGroundTruthAsync(
        string outputJson, string groundTruthPath)
    {
        return await _validator.CompareWithGroundTruthAsync(outputJson, groundTruthPath);
    }
}

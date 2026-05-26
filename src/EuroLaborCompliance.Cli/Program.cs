using EuroLaborCompliance.Pipeline;
using EuroLaborCompliance.Pipeline.Ocr;
using EuroLaborCompliance.Pipeline.Mapping;

var testDir = FindTestDir();
var docsDir = Path.Combine(testDir, "data", "documents");
var gtPath = Path.Combine(testDir, "data", "ground-truth", "tc-001-setu-output.json");

var pdf = Directory.GetFiles(docsDir, "*.pdf")
    .OrderBy(f => f.Contains("2026") ? 0 : 1)
    .FirstOrDefault();
var docPath = pdf ?? Path.Combine(docsDir, "tc-001-contract.txt");

Console.WriteLine("Euro Labor Compliance AI — LLM Pipeline");
Console.WriteLine("=======================================");
Console.WriteLine();

if (!File.Exists(docPath)) { Console.WriteLine($"[ERROR] File not found: {docPath}"); return 1; }

// API key from environment or inline
var apiKey = Environment.GetEnvironmentVariable("OPENCODE_GO_API_KEY")
          ?? "sk-eUJaZOpEOETTGqklFL1IvLmeOc6FmLCMlWSJMouUKJqJCsVqMPVeLRZegGbaq0DH";

Console.WriteLine($"[1/4] OCR: {Path.GetFileName(docPath)} ({new FileInfo(docPath).Length / 1024} KB)");
var ocr = new HuggingFaceOcrService();
var mapper = new LlmMapper(apiKey);

var pipeline = new CompliancePipeline(ocr, mapper);
var result = await pipeline.RunAsync(docPath);

Console.WriteLine($"[2/4] LLM mapping: confidence={result.MappingConfidence:P0}");
foreach (var flag in result.Flags)
    Console.WriteLine($"       [{flag.Type}] {flag.Field}: {flag.Message}");

var outputDir = Path.Combine(testDir, "..", "output");
Directory.CreateDirectory(outputDir);
var outputPath = Path.Combine(outputDir, $"llm-output-{DateTime.Now:yyyyMMdd-HHmmss}.json");
await File.WriteAllTextAsync(outputPath, result.OutputJson);
Console.WriteLine($"[3/4] Saved: {outputPath}");

if (File.Exists(gtPath))
{
    var report = await pipeline.ValidateAgainstGroundTruthAsync(result.OutputJson, gtPath);
    Console.WriteLine($"\n[4/4] Match Score: {report.MatchScore:P0} ({report.MatchedFields}/{report.TotalFieldsChecked})");
}

Console.WriteLine("\nDone.");
return 0;

static string FindTestDir()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "test", "data")))
        dir = dir.Parent;
    return dir != null ? Path.Combine(dir.FullName, "test") : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "test");
}

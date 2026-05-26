using EuroLaborCompliance.Pipeline;
using EuroLaborCompliance.Pipeline.Ocr;

var testDir = FindTestDir();
var docsDir = Path.Combine(testDir, "data", "documents");
var gtPath = Path.Combine(testDir, "data", "ground-truth", "tc-001-setu-output.json");

// Find first real PDF
var pdf = Directory.GetFiles(docsDir, "*.pdf")
    .OrderBy(f => f.Contains("2026") ? 0 : 1)
    .FirstOrDefault();

var docPath = pdf ?? Path.Combine(docsDir, "tc-001-contract.txt");

Console.WriteLine("Euro Labor Compliance AI - OCR Pipeline");
Console.WriteLine("========================================");
Console.WriteLine();

if (!File.Exists(docPath))
{
    Console.WriteLine($"[ERROR] File not found: {docPath}");
    return 1;
}

Console.WriteLine($"[1/4] OCR: {Path.GetFileName(docPath)} ({new FileInfo(docPath).Length / 1024} KB)");

IOcrService ocr = docPath.EndsWith(".pdf")
    ? new HuggingFaceOcrService()
    : new RuleBasedOcrService();

var pipeline = new CompliancePipeline(ocr);
var result = await pipeline.RunAsync(docPath);

Console.WriteLine($"[2/4] OCR complete: {result.OcrInfo?.Engine}, {result.OcrInfo?.PageCount} pages");
Console.WriteLine($"[3/4] Mapping: confidence={result.MappingConfidence:P0}, mapped={result.FieldsMapped}");

foreach (var flag in result.Flags)
    Console.WriteLine($"       [{flag.Type}] {flag.Field}: {flag.Message}");

var outputDir = Path.Combine(testDir, "..", "output");
Directory.CreateDirectory(outputDir);
var outputPath = Path.Combine(outputDir, $"ocr-output-{DateTime.Now:yyyyMMdd-HHmmss}.json");
await File.WriteAllTextAsync(outputPath, result.OutputJson);
Console.WriteLine($"[4/4] Saved: {outputPath}");

if (File.Exists(gtPath))
{
    var report = await pipeline.ValidateAgainstGroundTruthAsync(result.OutputJson, gtPath);
    Console.WriteLine($"\nMatch Score: {report.MatchScore:P0} | Matched: {report.MatchedFields}/{report.TotalFieldsChecked}");
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

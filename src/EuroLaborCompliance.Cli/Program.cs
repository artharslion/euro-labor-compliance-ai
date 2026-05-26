using EuroLaborCompliance.Pipeline;

var testDir = FindTestDir();
var docPath = Path.Combine(testDir, "data", "documents", "tc-001-contract.txt");
var gtPath = Path.Combine(testDir, "data", "ground-truth", "tc-001-setu-output.json");
var outputDir = Path.Combine(testDir, "..", "output");
Directory.CreateDirectory(outputDir);

Console.WriteLine("Euro Labor Compliance AI - PoC Pipeline");
Console.WriteLine("========================================");
Console.WriteLine();

if (!File.Exists(docPath)) { Console.WriteLine($"[ERROR] Document not found: {docPath}"); return 1; }

Console.WriteLine($"[1/3] Ingesting: {Path.GetFileName(docPath)}");
var pipeline = new CompliancePipeline();
var result = await pipeline.RunAsync(docPath);

Console.WriteLine($"[2/3] Mapping: confidence={result.MappingConfidence:P0}, mapped={result.FieldsMapped}, missing={result.FieldsMissing}");
foreach (var flag in result.Flags)
    Console.WriteLine($"       [{flag.Type}] {flag.Field}: {flag.Message}");

var outputPath = Path.Combine(outputDir, $"output-{DateTime.Now:yyyyMMdd-HHmmss}.json");
await File.WriteAllTextAsync(outputPath, result.OutputJson);
Console.WriteLine($"[3/3] Saved: {outputPath}");

if (File.Exists(gtPath))
{
    Console.WriteLine();
    var report = await pipeline.ValidateAgainstGroundTruthAsync(result.OutputJson, gtPath);
    Console.WriteLine($"Match Score: {report.MatchScore:P0} | Checked: {report.TotalFieldsChecked} | Matched: {report.MatchedFields} | Mismatches: {report.MismatchedFields} | Missing: {report.MissingFields}");
    Console.WriteLine($"Status: {(report.Passed ? "PASSED" : "FAILED")}");

    foreach (var diff in report.Differences.Take(10))
        Console.WriteLine($"  {diff.Path}: expected={Trunc(diff.Expected,50)} actual={Trunc(diff.Actual,50)}");
}
else
    Console.WriteLine("[SKIP] No ground truth file");

Console.WriteLine("\nDone.");
return 0;

static string FindTestDir()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "test", "data")))
        dir = dir.Parent;
    return dir != null ? Path.Combine(dir.FullName, "test") : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "test");
}
static string Trunc(string s, int n) => s.Length <= n ? s : s[..(n - 3)] + "...";

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EuroLaborCompliance.Pipeline.Ocr;

public class HuggingFaceOcrService : IOcrService
{
    private readonly HttpClient _http;
    private const string HfInferenceUrl = "https://api-inference.huggingface.co/models/deepseek-ai/DeepSeek-OCR";

    public HuggingFaceOcrService(HttpClient? http = null)
    {
        _http = http ?? new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
    }

    public async Task<OcrResult> ExtractAsync(string filePath)
    {
        var sw = Stopwatch.StartNew();
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        try
        {
            using var content = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(ext == ".pdf" ? "application/pdf" : "image/png");
            content.Add(fileContent, "file", Path.GetFileName(filePath));
            var response = await _http.PostAsync(HfInferenceUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var text = HfText(doc.RootElement);
                return new OcrResult(Path.GetFileName(filePath), text, text, "HuggingFace", Pages(text), sw.Elapsed.TotalSeconds);
            }
        }
        catch { }

        return await DualExtractAsync(filePath, sw);
    }

    private async Task<OcrResult> DualExtractAsync(string filePath, Stopwatch sw)
    {
        var tmp = Path.Combine(Path.GetTempPath(), "setu-ocr");
        Directory.CreateDirectory(tmp);
        var outPath = Path.Combine(tmp, $"{Path.GetFileNameWithoutExtension(filePath)}.md");
        var scriptPath = FindScript();

        var psi = new ProcessStartInfo
        {
            FileName = @"C:\Users\zhixi\AppData\Local\Python\bin\python3.exe",
            Arguments = $@"""{scriptPath}"" ""{filePath}"" ""{outPath}"" 0 80000",
            RedirectStandardOutput = true, RedirectStandardError = true,
            UseShellExecute = false, CreateNoWindow = true
        };

        try
        {
            using var proc = Process.Start(psi)!;
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            await proc.WaitForExitAsync(cts.Token);
            var stdout = await proc.StandardOutput.ReadToEndAsync();
            var stderr = await proc.StandardError.ReadToEndAsync();

            if (File.Exists(outPath) && new FileInfo(outPath).Length > 100)
            {
                var md = await File.ReadAllTextAsync(outPath);
                int pages = 1;
                try { var j = JsonDocument.Parse(stdout); pages = int.Parse(j.RootElement.GetProperty("pages_processed").GetInt32().ToString()); }
                catch { pages = md.Split("\n## Page").Length; }
                return new OcrResult(Path.GetFileName(filePath), md, md, "DualExtract", pages, sw.Elapsed.TotalSeconds);
            }

            if (!string.IsNullOrWhiteSpace(stderr))
                throw new Exception(stderr.Length > 300 ? stderr[..300] : stderr);
        }
        catch (Exception ex) { throw new Exception($"OCR failed: {ex.Message}"); }

        throw new Exception("OCR produced no output");
    }

    private static string HfText(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            var f = root[0];
            if (f.TryGetProperty("generated_text", out var g)) return g.GetString() ?? "";
            if (f.TryGetProperty("summary_text", out var s)) return s.GetString() ?? "";
        }
        if (root.TryGetProperty("generated_text", out var gt)) return gt.GetString() ?? "";
        if (root.TryGetProperty("text", out var t)) return t.GetString() ?? "";
        return root.ToString();
    }

    private static string FindScript()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d != null && !File.Exists(Path.Combine(d.FullName, "test", "dual_extract.py"))) d = d.Parent;
        return d != null ? Path.Combine(d.FullName, "test", "dual_extract.py") :
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "test", "dual_extract.py");
    }

    private static int Pages(string t) => t.Split("\n## Page").Length;
}

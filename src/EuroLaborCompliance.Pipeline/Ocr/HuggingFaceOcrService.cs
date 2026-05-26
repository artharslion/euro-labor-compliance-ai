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

        // Try HuggingFace inference first (no auth for public models)
        try
        {
            using var content = new MultipartFormDataContent();
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                ext == ".pdf" ? "application/pdf" : "image/png");
            content.Add(fileContent, "file", Path.GetFileName(filePath));

            var response = await _http.PostAsync(HfInferenceUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var text = ExtractTextFromHfResponse(doc.RootElement);

                return new OcrResult(
                    Path.GetFileName(filePath), text, text, "HuggingFace-DeepSeek-OCR",
                    PageCount(text), sw.Elapsed.TotalSeconds
                );
            }
        }
        catch { /* fall through to Python subprocess */ }

        // Fallback: Python deepseek-ocr package
        return await PythonDeepSeekOcrAsync(filePath, sw);
    }

    private async Task<OcrResult> PythonDeepSeekOcrAsync(string filePath, Stopwatch sw)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "setu-ocr");
        Directory.CreateDirectory(tempDir);
        var outputPath = Path.Combine(tempDir, $"{Path.GetFileNameWithoutExtension(filePath)}.md");

        // Use pymupdf for text extraction + markdown conversion (free, no API key)
        var psi = new ProcessStartInfo
        {
            FileName = @"C:\Users\zhixi\AppData\Local\Python\bin\python3.exe",
            Arguments = $"-c \"import fitz; doc=fitz.open(r'{filePath}'); pages=[]; [pages.append(f'## Page {{i+1}}\\n\\n{{page.get_text()}}\\n') for i,page in enumerate(doc)]; open(r'{outputPath}','w',encoding='utf-8').write('\\n'.join(pages))\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi)!;
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            await process.WaitForExitAsync(cts.Token);
            var error = await process.StandardError.ReadToEndAsync();

            if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 100)
            {
                var markdown = await File.ReadAllTextAsync(outputPath);
                return new OcrResult(
                    Path.GetFileName(filePath), markdown, markdown,
                    "PyMuPDF-OCR", PageCount(markdown), sw.Elapsed.TotalSeconds
                );
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                var shortError = error.Length > 300 ? error[..300] : error;
                throw new Exception($"Python OCR failed: {shortError}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"All OCR backends failed for {filePath}. Last error: {ex.Message}");
        }

        throw new Exception($"OCR produced no output for {filePath}");
    }

    private static string ExtractTextFromHfResponse(JsonElement root)
    {
        // HF inference returns different formats depending on model
        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            var first = root[0];
            if (first.TryGetProperty("generated_text", out var gt))
                return gt.GetString() ?? "";
            if (first.TryGetProperty("summary_text", out var st))
                return st.GetString() ?? "";
        }
        if (root.TryGetProperty("generated_text", out var g))
            return g.GetString() ?? "";
        if (root.TryGetProperty("text", out var t))
            return t.GetString() ?? "";
        return root.ToString();
    }

    private static int PageCount(string text) =>
        text.Split("\n---\n", StringSplitOptions.None).Length;
}

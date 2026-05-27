using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace EuroLaborCompliance.Pipeline.Ocr;

/// <summary>
/// DeepSeek OCR 2 integration via REST API (deepseek-ocr-2-client Docker).
/// Endpoint defaults to localhost:20000, override via constructor or env var DEEPSEEK_OCR2_URL.
/// </summary>
public class DeepSeekOcr2Service : IOcrService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public DeepSeekOcr2Service(string? baseUrl = null, HttpClient? http = null)
    {
        _baseUrl = baseUrl
            ?? Environment.GetEnvironmentVariable("DEEPSEEK_OCR2_URL")
            ?? "http://localhost:20000";
        _http = http ?? new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
    }

    public async Task<OcrResult> ExtractAsync(string filePath)
    {
        var sw = Stopwatch.StartNew();
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        // PDF: use /parse endpoint for full document → markdown
        if (ext == ".pdf")
        {
            return await ParsePdfAsync(filePath, sw);
        }

        // Image: use /v1/ocr endpoint
        return await OcrImageAsync(filePath, sw);
    }

    private async Task<OcrResult> ParsePdfAsync(string filePath, Stopwatch sw)
    {
        using var content = new MultipartFormDataContent();
        var bytes = await File.ReadAllBytesAsync(filePath);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "file", Path.GetFileName(filePath));
        content.Add(new StringContent("json"), "type");

        var response = await _http.PostAsync($"{_baseUrl}/parse", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var markdown = root.TryGetProperty("markdown", out var md)
            ? md.GetString() ?? ""
            : json;
        var pages = root.TryGetProperty("pages", out var p) ? p.GetInt32() : 1;

        return new OcrResult(Path.GetFileName(filePath), markdown, markdown,
            "DeepSeek-OCR-2", pages, sw.Elapsed.TotalSeconds);
    }

    private async Task<OcrResult> OcrImageAsync(string filePath, Stopwatch sw)
    {
        using var content = new MultipartFormDataContent();
        var bytes = await File.ReadAllBytesAsync(filePath);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        content.Add(fileContent, "file", Path.GetFileName(filePath));
        content.Add(new StringContent("<image>\n<|grounding|>Convert the document to markdown."), "prompt");

        var response = await _http.PostAsync($"{_baseUrl}/v1/ocr", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var text = JsonDocument.Parse(json).RootElement.TryGetProperty("text", out var t)
            ? t.GetString() ?? json
            : json;

        return new OcrResult(Path.GetFileName(filePath), text, text,
            "DeepSeek-OCR-2", 1, sw.Elapsed.TotalSeconds);
    }
}

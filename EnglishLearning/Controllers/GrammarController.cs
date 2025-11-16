using System.Net.Http;
using System.Text;
using System.Text.Json;
using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;

public class GrammarController : Controller
{
    private readonly HttpClient _httpClient;

    public GrammarController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public IActionResult Check()
    {
        return View(new GrammarCheckViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Check(GrammarCheckViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.InputText))
        {
            ModelState.AddModelError("", "Vui lòng nhập văn bản để kiểm tra!");
            return View(model);
        }

        try
        {
            var data = new Dictionary<string, string>
            {
                { "text", model.InputText },
                { "language", "en-US" }
            };
            var content = new FormUrlEncodedContent(data);

            var response = await _httpClient.PostAsync("https://api.languagetool.org/v2/check", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(result);
            var matches = doc.RootElement.GetProperty("matches").EnumerateArray();

            model.Errors.Clear();
            model.CorrectedText = model.InputText; // Khởi tạo văn bản đã sửa bằng văn bản gốc
            model.ErrorDetails.Clear();
            int offsetAdjustment = 0; // Theo dõi sự thay đổi offset do sửa lỗi

            foreach (var match in matches)
            {
                var message = match.GetProperty("message").GetString();
                var offset = match.GetProperty("offset").GetInt32();
                var length = match.GetProperty("length").GetInt32();

                // Điều chỉnh offset dựa trên các thay đổi trước đó
                int adjustedOffset = offset + offsetAdjustment;

                int contextLength = 10;
                int start = Math.Max(0, adjustedOffset - contextLength);
                int end = Math.Min(model.InputText.Length, adjustedOffset + length + contextLength);
                var contextText = model.InputText.Substring(start, end - start);
                string prefix = start > 0 ? "..." : "";
                string suffix = end < model.InputText.Length ? "..." : "";

                var replacements = match.GetProperty("replacements");
                string suggestions = "";
                string firstSuggestion = null;
                if (replacements.ValueKind == JsonValueKind.Array && replacements.GetArrayLength() > 0)
                {
                    var suggestedWords = new List<string>();
                    foreach (var replacement in replacements.EnumerateArray())
                    {
                        suggestedWords.Add(replacement.GetProperty("value").GetString());
                    }
                    suggestions = string.Join(", ", suggestedWords);
                    firstSuggestion = suggestedWords[0]; // Lấy gợi ý đầu tiên
                }

                string errorString = $"Lỗi: {message} → \"{prefix}{contextText}{suffix}\"";
                if (!string.IsNullOrEmpty(suggestions))
                {
                    errorString += $" (Gợi ý: {suggestions})";
                }
                model.Errors.Add(errorString);

                // Lưu chi tiết lỗi với offset đã điều chỉnh
                var errorDetail = new ErrorDetail
                {
                    OriginalOffset = offset, // Sử dụng offset ban đầu từ API
                    OriginalLength = length,
                    CorrectedOffset = adjustedOffset, // Vị trí sau khi áp dụng các sửa trước
                    CorrectedLength = firstSuggestion?.Length ?? length,
                    IsCorrected = !string.IsNullOrEmpty(firstSuggestion),
                    Suggestion = firstSuggestion
                };
                model.ErrorDetails.Add(errorDetail);

                // Áp dụng gợi ý đầu tiên vào văn bản đã sửa
                if (firstSuggestion != null)
                {
                    model.CorrectedText = model.CorrectedText.Remove(adjustedOffset, length).Insert(adjustedOffset, firstSuggestion);
                    offsetAdjustment += (firstSuggestion.Length - length); // Cập nhật offsetAdjustment
                }
            }

          
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError("", $"Lỗi kết nối đến API: {ex.Message}");
        }
        catch (JsonException ex)
        {
            ModelState.AddModelError("", $"Lỗi phân tích dữ liệu: {ex.Message}");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Lỗi không xác định: {ex.Message}");
        }

        return View(model);
    }
}
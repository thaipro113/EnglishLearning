using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EnglishLearning.Controllers
{
    [Route("Translation")]
    public class TranslationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; private readonly IConfiguration _configuration; private readonly EnglishLearningDbContext _context;

        public TranslationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, EnglishLearningDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
        }

        // --- VIEW 1: Chọn cấp độ & chủ đề ---
        [HttpGet("SelectLevel")]
        public IActionResult SelectLevel()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Translation/SelectLevel" });
            return View();
        }

        [HttpPost("StartPractice")]
        public async Task<IActionResult> StartPractice(string Level, string Topic)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Translation/SelectLevel" });

            var apiKey = _configuration["Groq:ApiKey"];
            var paragraph = await CallGroqApi(
                $"Tạo 1 đoạn văn tiếng Anh gồm 10 câu, mỗi câu không quá ngắn, kết thúc bằng dấu chấm, không đánh số thứ tự, theo chính xác cấp độ {Level} và chủ đề {Topic}.",
                apiKey);

            var sentences = paragraph.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim() + ".")
                .Where(s => s.Length > 1)
                .Take(10)
                .ToList();

            if (sentences.Count < 10)
            {
                return BadRequest("Đoạn văn không đủ 10 câu. Vui lòng thử lại.");
            }

            var standardTranslations = new List<string>();
            foreach (var sentence in sentences)
            {
                var translateRequest = $"Dịch câu sau sang tiếng Việt một cách tự nhiên và chính xác: {sentence}";
                var translateReply = await CallGroqApi(translateRequest, apiKey);
                standardTranslations.Add(translateReply.Trim());
            }

            // Save paragraph to database
            var paragraphEntity = new Paragraph
            {
                UserId = userId.Value,
                Level = Level,
                Topic = Topic,
                ParagraphText = paragraph,
                Timestamp = DateTime.UtcNow
            };
            _context.Paragraphs.Add(paragraphEntity);
            await _context.SaveChangesAsync();

            var paragraphData = new ParagraphData
            {
                Level = Level,
                Topic = Topic,
                Paragraph = paragraph,
                Sentences = sentences,
                StandardTranslations = standardTranslations
            };

            HttpContext.Session.SetString("CurrentParagraph", JsonSerializer.Serialize(paragraphData));
            HttpContext.Session.SetInt32("CurrentSentenceIndex", 0);
            HttpContext.Session.SetInt32("CurrentParagraphId", paragraphEntity.Id);
            ViewBag.CurrentSentenceIndex = 0;
            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Practice", new { paragraphId = paragraphEntity.Id })
            });
        }
        [HttpGet("Practice")]
        public IActionResult Practice(int paragraphId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Translation/SelectLevel" });

            var paragraphJson = HttpContext.Session.GetString("CurrentParagraph");
            if (string.IsNullOrEmpty(paragraphJson))
                return RedirectToAction("SelectLevel");

            var paragraphData = JsonSerializer.Deserialize<ParagraphData>(paragraphJson);
            ViewBag.CurrentSentenceIndex = HttpContext.Session.GetInt32("CurrentSentenceIndex") ?? 0;

            return View(paragraphData);
        }

        // --- VIEW 2: Practice - Kiểm tra bản dịch ---
        [HttpPost("CheckTranslation")]
        public async Task<IActionResult> CheckTranslation([FromBody] TranslationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.UserTranslation))
                return Json(new { success = false, error = "Vui lòng nhập bản dịch!" });

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { success = false, error = "Bạn chưa đăng nhập!" });

            var paragraphJson = HttpContext.Session.GetString("CurrentParagraph");
            if (string.IsNullOrEmpty(paragraphJson))
                return Json(new { success = false, error = "Phiên làm bài đã hết hạn. Vui lòng bắt đầu lại." });

            var paragraphData = JsonSerializer.Deserialize<ParagraphData>(paragraphJson);
            if (paragraphData == null)
                return Json(new { success = false, error = "Dữ liệu không hợp lệ!" });

            var currentIndex = HttpContext.Session.GetInt32("CurrentSentenceIndex") ?? 0;
            if (currentIndex >= paragraphData.Sentences.Count)
                return Json(new { success = false, error = "Đã hoàn thành đoạn văn!" });

            var paragraphId = HttpContext.Session.GetInt32("CurrentParagraphId") ?? 0;
            var userTranslation = request.UserTranslation.Trim();
            var standardTranslation = paragraphData.StandardTranslations[currentIndex].Trim();
            var originalSentence = paragraphData.Sentences[currentIndex];

            // Đánh giá ngữ nghĩa bằng API Groq
            var evaluationRequest = $@"Đánh giá mức độ bằng tiếng việt tương đồng ngữ nghĩa giữa hai bản dịch tiếng Việt sau, trả về một số từ 0 đến 100 (điểm càng cao nghĩa càng giống), và một gợi ý ngắn gọn (dưới 35 từ) về cách sửa hoặc lỗi chính cần khắc phục:

Bản dịch chuẩn: {standardTranslation} Bản dịch người dùng: {userTranslation} Định dạng trả về: {{ ""accuracy"": số, ""errorSuggestion"": ""gợi ý"" }}";

            var evaluationResponse = await CallGroqApi(evaluationRequest, _configuration["Groq:ApiKey"]);
            double accuracy;
            string errorSuggestion;

            try
            {
                var evaluationResult = JsonSerializer.Deserialize<JsonElement>(evaluationResponse);
                accuracy = evaluationResult.GetProperty("accuracy").GetDouble();
                errorSuggestion = evaluationResult.GetProperty("errorSuggestion").GetString() ?? "Không có gợi ý cụ thể.";
            }
            catch
            {
                accuracy = 0;
                errorSuggestion = "Lỗi khi đánh giá bản dịch. Vui lòng thử lại.";
            }

            // Lưu vào database
            var history = new TranslationHistory
            {
                UserId = userId.Value,
                ParagraphId = paragraphId,
                Topic = paragraphData.Topic,
                OriginalSentence = originalSentence,
                UserTranslation = userTranslation,
                StandardTranslation = standardTranslation,
                Accuracy = Math.Round(accuracy, 2),
                ErrorSuggestion = errorSuggestion,
                Timestamp = DateTime.UtcNow
            };
            _context.TranslationHistories.Add(history);
            await _context.SaveChangesAsync();

            // Chuyển sang câu tiếp theo
            currentIndex++;
            HttpContext.Session.SetInt32("CurrentSentenceIndex", currentIndex);
            var nextSentence = currentIndex < paragraphData.Sentences.Count ? paragraphData.Sentences[currentIndex] : null;

            return Json(new
            {
                success = true,
                accuracy = Math.Round(accuracy, 2),
                errorSuggestion,
                nextSentence,
                isComplete = currentIndex >= paragraphData.Sentences.Count
            });
        }

        // --- VIEW 3: Lịch sử (Summary of Paragraphs) ---
        [HttpGet("History")]
        public async Task<IActionResult> History()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Translation/History" });

            var paragraphs = await _context.Paragraphs
                .Where(p => p.UserId == userId.Value)
                .OrderByDescending(p => p.Timestamp)
                .ToListAsync();

            var result = paragraphs.Select(p => new
            {
                p.Id,
                p.Level,
                p.Topic,
                p.ParagraphText,
                p.Timestamp,
                SentenceCount = _context.TranslationHistories.Count(h => h.ParagraphId == p.Id),
                TotalSentences = p.ParagraphText.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .Take(10)
                    .Count()
            }).ToList();

            return View("ParagraphHistory", result);
        }

        // --- VIEW 4: Chi tiết lịch sử của một đoạn văn ---
        [HttpGet("ParagraphHistory/{paragraphId}")]
        public async Task<IActionResult> ParagraphHistory(int paragraphId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Translation/ParagraphHistory/{paragraphId}" });

            var history = await _context.TranslationHistories
                .Where(h => h.UserId == userId.Value && h.ParagraphId == paragraphId)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();

            var paragraph = await _context.Paragraphs
                .Where(p => p.Id == paragraphId && p.UserId == userId.Value)
                .FirstOrDefaultAsync();

            if (paragraph == null)
                return NotFound("Đoạn văn không tồn tại hoặc không thuộc về bạn.");

            var totalSentences = paragraph.ParagraphText.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Take(10)
                .Count();

            ViewBag.ParagraphText = paragraph.ParagraphText;
            ViewBag.Topic = paragraph.Topic;
            ViewBag.Level = paragraph.Level;
            ViewBag.SentenceCount = _context.TranslationHistories.Count(h => h.ParagraphId == paragraphId);
            ViewBag.TotalSentences = totalSentences;
            ViewBag.IsComplete = ViewBag.SentenceCount >= totalSentences;
            ViewBag.CurrentParagraphId = paragraphId;

            return View("History", history);
        }
        [HttpGet("ContinuePractice/{paragraphId}")]
        public async Task<IActionResult> ContinuePractice(int paragraphId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Translation/ContinuePractice/{paragraphId}" });

            var paragraph = await _context.Paragraphs
                .FirstOrDefaultAsync(p => p.Id == paragraphId && p.UserId == userId.Value);

            if (paragraph == null)
                return NotFound("Đoạn văn không tồn tại hoặc không thuộc về bạn.");

            var history = await _context.TranslationHistories
                .Where(h => h.ParagraphId == paragraphId && h.UserId == userId.Value)
                .ToListAsync();

            var sentences = paragraph.ParagraphText.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim() + ".")
                .Where(s => s.Length > 1)
                .Take(10)
                .ToList();

            if (sentences.Count == 0)
                return BadRequest("Đoạn văn không có câu hợp lệ.");

            // 👉 Không gọi API dịch lại, mà dùng bản dịch chuẩn từ DB
            var apiKey = _configuration["Groq:ApiKey"];
            var standardTranslations = new List<string>();

            foreach (var sentence in sentences)
            {
                var translation = history.FirstOrDefault(h => h.OriginalSentence == sentence)?.StandardTranslation;

                if (string.IsNullOrEmpty(translation))
                {
                    // Gọi lại API để dịch chuẩn cho câu chưa có
                    var translateRequest = $"Dịch câu sau sang tiếng Việt một cách tự nhiên và chính xác: {sentence}";
                    translation = await CallGroqApi(translateRequest, apiKey);
                }

                standardTranslations.Add(translation.Trim());
            }


            var paragraphData = new ParagraphData
            {
                Level = paragraph.Level,
                Topic = paragraph.Topic,
                Paragraph = paragraph.ParagraphText,
                Sentences = sentences,
                StandardTranslations = standardTranslations
            };

            var currentSentenceIndex = history.Count; // số câu đã làm
            HttpContext.Session.SetString("CurrentParagraph", JsonSerializer.Serialize(paragraphData));
            HttpContext.Session.SetInt32("CurrentSentenceIndex", currentSentenceIndex);
            HttpContext.Session.SetInt32("CurrentParagraphId", paragraph.Id);

            ViewBag.CurrentSentenceIndex = currentSentenceIndex;

            return View("Practice", paragraphData);
        }


        [HttpPost("ClearHistory")]
        public async Task<IActionResult> ClearHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { error = "Bạn chưa đăng nhập!" });

            var paragraphs = _context.Paragraphs.Where(p => p.UserId == userId);
            _context.Paragraphs.RemoveRange(paragraphs);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa toàn bộ lịch sử." });
        }

        [HttpPost("DeleteParagraph/{paragraphId}")]
        public async Task<IActionResult> DeleteParagraph(int paragraphId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Json(new { error = "Bạn chưa đăng nhập!" });

            var paragraph = await _context.Paragraphs
                .Where(p => p.Id == paragraphId && p.UserId == userId.Value)
                .FirstOrDefaultAsync();

            if (paragraph == null)
                return Json(new { error = "Đoạn văn không tồn tại hoặc không thuộc về bạn." });

            // Xóa tất cả lịch sử dịch liên quan đến ParagraphId
            var histories = await _context.TranslationHistories
                .Where(h => h.ParagraphId == paragraphId)
                .ToListAsync();
            _context.TranslationHistories.RemoveRange(histories);

            // Xóa đoạn văn
            _context.Paragraphs.Remove(paragraph);
            await _context.SaveChangesAsync();

            return Json(new { message = "Đã xóa đoạn văn thành công!" });
        }
        // --- Call API Groq ---
        private async Task<string> CallGroqApi(string input, string apiKey, int maxRetries = 3)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var payload = new
            {
                model = "openai/gpt-oss-20b",
                messages = new[] { new { role = "user", content = input } }
            };

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", payload);
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<JsonElement>(data);
                    return json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
                }
                catch (HttpRequestException ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"[DEBUG] API call failed, retry {attempt}/{maxRetries}: {ex.Message}");
                    await Task.Delay(1000 * attempt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] API call error: {ex.Message}");
                    throw;
                }
            }
            throw new Exception("Failed to call Groq API after retries.");
        }
    }

    public class TranslationRequest
    {
        public string UserTranslation { get; set; }
    }

}
using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly EnglishLearningDbContext _context;

        public ChatbotController(IHttpClientFactory httpClientFactory, IConfiguration configuration, EnglishLearningDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Chatbot" });

            var history = await _context.ChatHistories
                .Where(h => h.UserId == userId.Value)
                .OrderBy(h => h.Timestamp)
                .ToListAsync();

            return View(history);
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.userInput))
                return Json(new { reply = "Vui lòng nhập câu hỏi!" });

            try
            {
                var apiKey = _configuration["OpenRouter:ApiKey"];
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                    return Json(new { reply = "Bạn chưa đăng nhập!" });

                _context.ChatHistories.Add(new ChatHistory
                {
                    UserId = userId.Value,
                    Role = "user",
                    Message = request.userInput,
                    Timestamp = DateTime.UtcNow
                });

                var reply = await CallOpenRouterApi(request.userInput, apiKey);

                _context.ChatHistories.Add(new ChatHistory
                {
                    UserId = userId.Value,
                    Role = "assistant",
                    Message = reply,
                    Timestamp = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();

                return Json(new { reply });
            }
            catch (Exception ex)
            {
                return Json(new { reply = $"Lỗi: {ex.Message}" });
            }
        }

        private async Task<string> CallOpenRouterApi(string input, string apiKey)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:44335");
            client.DefaultRequestHeaders.Add("X-Title", "WSP ChatBot");

            var payload = new
            {
                model = "openai/gpt-oss-20b:free",
                messages = new[] { new { role = "user", content = input } }
            };

            var response = await client.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", payload);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            var jsonData = JsonSerializer.Deserialize<JsonElement>(data);
            return jsonData.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
        [HttpPost]
        public async Task<IActionResult> ClearHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); ; // hàm lấy ID người dùng
            var userMessages = _context.ChatHistories.Where(m => m.UserId == userId);

            _context.ChatHistories.RemoveRange(userMessages);
            await _context.SaveChangesAsync();

            return Json(new { message = "Đã xóa toàn bộ lịch sử chat." });
        }

    }

    public class ChatRequest
    {
        public string userInput { get; set; }
    }
}

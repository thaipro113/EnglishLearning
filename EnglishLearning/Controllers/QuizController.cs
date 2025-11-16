using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishLearning.Models; // <-- nếu có model Quiz ở đây
using System;
using System.Threading.Tasks;
using System.Text;

namespace EnglishLearning.Controllers
{
    public class QuizController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public QuizController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return View(quizzes);
        }

        [HttpPost]
        public async Task<IActionResult> CheckAnswer([FromBody] QuizAnswerDto dto)
        {
            var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
            if (quiz == null)
            {
                return Json(new { success = false, message = "Không tìm thấy câu hỏi." });
            }

            // Chuẩn hóa
            string? selectedAnswer = dto.SelectedAnswer?.Trim().ToLowerInvariant();
            string? correctAnswer = quiz.CorrectAnswer?.Trim().ToLowerInvariant();


            bool isCorrect = selectedAnswer == correctAnswer;

            return Json(new
            {
                success = true,
                isCorrect,
                correctAnswer = quiz.CorrectAnswer
            });
        }

    }

    // DTO để nhận dữ liệu từ JavaScript
    public class QuizAnswerDto
    {
        public int QuizId { get; set; }
        public string SelectedAnswer { get; set; } = string.Empty;
    }
}

using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Controllers
{
    public class LessonsController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public LessonsController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        public IActionResult View(string id)
        {
            var lesson = _context.Lessons
                .Include(l => l.Quizzes) // Bao gồm Quizzes để hiển thị trong view
                .FirstOrDefault(l => l.LessonId == id);
            if (lesson == null) return NotFound();

            return View(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(string lessonId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var progress = await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

            if (progress == null)
            {
                progress = new Progress
                {
                    UserId = userId.Value,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.Now
                };
                _context.Progresses.Add(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.Now;
                _context.Progresses.Update(progress);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Course", new { id = progress.Lesson.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> CheckTestAnswers([FromBody] List<QuizAnswerModel> answers)
        {
            var results = new List<object>();
            foreach (var answer in answers)
            {
                var quiz = await _context.Quizzes
                    .FirstOrDefaultAsync(q => q.QuizId == answer.QuizId);
                if (quiz == null) continue;

                var isCorrect = quiz.CorrectAnswer == answer.SelectedAnswer;
                results.Add(new
                {
                    quizId = answer.QuizId,
                    isCorrect,
                    correctAnswer = quiz.CorrectAnswer
                });
            }

            return Json(new
            {
                success = true,
                results
            });
        }
    }

    public class QuizAnswerModel
    {
        public int QuizId { get; set; }
        public string SelectedAnswer { get; set; }
    }
}
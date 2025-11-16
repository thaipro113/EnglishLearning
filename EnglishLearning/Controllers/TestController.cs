using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers
{
    public class TestController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public TestController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        // GET: Test/DoTest/lessonId
        public IActionResult DoTest(string lessonId)
        {
            if (string.IsNullOrEmpty(lessonId))
            {
                return BadRequest("Lesson ID is required.");
            }

            var lesson = _context.Lessons
                .Include(l => l.Quizzes)
                .Include(l => l.Course)
                .FirstOrDefault(l => l.LessonId == lessonId);

            if (lesson == null || lesson.Quizzes == null || !lesson.Quizzes.Any())
            {
                return NotFound("Lesson or quizzes not found.");
            }

            var quizzesQuery = lesson.Quizzes
                .Where(q => !q.IsDeleted);

            List<Quiz> quizzes;

            if (lesson.LessonType == "TOEIC")
            {
                // Không random, giữ nguyên thứ tự (theo QuizId hoặc OrderIndex)
                quizzes = quizzesQuery.OrderBy(q => q.QuizId).ToList();
            }
            else
            {
                // Random
                var random = new Random();
                quizzes = quizzesQuery.OrderBy(q => random.Next()).ToList();
            }

            // Debug để kiểm tra giá trị Title
            Console.WriteLine($"[DEBUG] LessonId: {lessonId}, Title: {lesson.Title ?? "null"}");

            // Gán Title vào ViewBag.Title
            ViewBag.Title = lesson.Title ?? "Bài kiểm tra không tên";
            ViewBag.LessonId = lessonId;
            ViewBag.CourseId = lesson.CourseId;
            ViewBag.LessonType = lesson.LessonType;
            ViewBag.AudioPath = lesson?.AudioPath;
            return View("DoTest", quizzes); // Chỉ định rõ tên view
        }

        // POST: Test/SubmitTest
        [HttpPost]
        public IActionResult SubmitTest(string lessonId, [FromForm] Dictionary<int, string> userAnswers)
        {
            Console.WriteLine($"[DEBUG] SubmitTest called with lessonId: {lessonId}, userAnswers count: {userAnswers?.Count ?? 0}");

            if (string.IsNullOrEmpty(lessonId) || userAnswers == null || !userAnswers.Any())
            {
                Console.WriteLine("[DEBUG] Invalid input data detected.");
                return BadRequest("Invalid input data. Please ensure all answers are selected.");
            }

            var lesson = _context.Lessons
                .Include(l => l.Quizzes)
                .FirstOrDefault(l => l.LessonId == lessonId);

            if (lesson == null)
            {
                Console.WriteLine("[DEBUG] Lesson not found for lessonId.");
                return NotFound("Lesson not found.");
            }

            var quizzes = lesson.Quizzes
                .Where(q => !q.IsDeleted)
                .ToList();

            Console.WriteLine($"[DEBUG] Số câu hỏi trong SubmitTest cho lessonId {lessonId}: {quizzes.Count}");

            if (quizzes == null || !quizzes.Any())
            {
                Console.WriteLine("[DEBUG] Quizzes not found for lessonId.");
                return NotFound("Quizzes not found.");
            }

            int correctCount = 0;
            var userAnswerDetails = new Dictionary<int, string>();
            int totalQuestions = quizzes.Count; // Số câu hỏi thực tế

            foreach (var quiz in quizzes)
            {
                if (userAnswers.TryGetValue(quiz.QuizId, out var answer))
                {
                    userAnswerDetails[quiz.QuizId] = answer;
                    if (quiz.CorrectAnswer.Equals(answer, StringComparison.OrdinalIgnoreCase))
                    {
                        correctCount++;
                    }
                }
                else
                {
                    Console.WriteLine($"[DEBUG] No answer found for QuizId: {quiz.QuizId}");
                }
            }

            int? userId = (int?)HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                Console.WriteLine("[DEBUG] User session not found.");
                return Unauthorized("User session not found. Please log in again.");
            }

            var progress = _context.Progresses
                .FirstOrDefault(p => p.UserId == userId.Value && p.LessonId == lessonId);

            float score;
            if (lesson.LessonType == "TOEIC")
            {
                // Thang điểm TOEIC: 990 điểm cho 200 câu (dựa trên số câu thực tế, tối đa 200)
                score = (float)correctCount / Math.Min(totalQuestions, 200) * 990;
            }
            else
            {
                // Thang điểm Normal: 10 điểm dựa trên tổng số câu hỏi
                score = (float)correctCount / totalQuestions * 10;
            }

            if (progress == null)
            {
                progress = new Progress
                {
                    UserId = userId.Value,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.Now,
                    Score = score // Lưu điểm theo thang điểm tương ứng
                };
                _context.Progresses.Add(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.Now;
                progress.Score = score; // Cập nhật điểm
            }

            try
            {
                _context.SaveChanges();
                Console.WriteLine("[DEBUG] Progress saved successfully with score: {progress.Score}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Error saving progress: {ex.Message}");
                return StatusCode(500, "An error occurred while saving progress.");
            }

            // Lấy lại lesson để gán ViewBag.Title cho Result view
            lesson = _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefault(l => l.LessonId == lessonId);
            ViewBag.Title = lesson?.Title ?? "Kết quả bài kiểm tra";
            ViewBag.Score = correctCount; // Truyền số câu đúng để view hiển thị
            ViewBag.UserAnswers = userAnswerDetails;
            ViewBag.TotalQuestions = totalQuestions; // Truyền tổng số câu hỏi để hiển thị
            ViewBag.CourseId = lesson?.CourseId;
            ViewBag.LessonType = lesson?.LessonType; // Truyền LessonType để view tính điểm đúng
            return View("Result", quizzes);
        }
    }
}
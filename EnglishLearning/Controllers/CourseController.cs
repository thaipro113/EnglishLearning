using EnglishLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Controllers
{
    public class CourseController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public CourseController(EnglishLearningDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Course" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null || string.IsNullOrEmpty(user.Level))
                return View(new List<Course>()); // Hoặc return thông báo không tìm thấy level

            // Lấy danh sách khóa học dựa trên SubLevel.LevelName khớp với user.Level
            var courses = await _context.Courses
                .Include(c => c.SubLevel)
                .ThenInclude(sl => sl.Level) // Bao gồm Level của SubLevel để kiểm tra LevelName
                .Where(c => c.SubLevel != null && c.SubLevel.Level != null && c.SubLevel.Level.LevelName == user.Level)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> Details(string id) // Thay int id thành string id
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Course/Details/{id}" });

            var course = await _context.Courses
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.Quizzes)
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.Progresses)
                .FirstOrDefaultAsync(c => c.CourseId == id); // So sánh với CourseId kiểu string

            if (course == null)
                return NotFound();

            ViewBag.UserId = userId;
            return View(course);
        }

    }
}
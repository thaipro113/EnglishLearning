using Microsoft.AspNetCore.Mvc;
using EnglishLearning.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public HomeController(EnglishLearningDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IActionResult Index()
        {
            // Đếm số lượng người dùng
            var userCount = _context.Users.Count();

            // Đếm số lượng khóa học
            var courseCount = _context.Courses.Count();

            // Đếm số lượng bài học
            var lessonCount = _context.Lessons.Count();

            // Đếm số lượng tiến trình hoàn thành
            var progressCount = _context.Progresses.Count(p => p.IsCompleted);

            // Tính phần trăm tiến trình hoàn thành
            var totalProgressPossible = userCount * lessonCount * 1.0;
            var progressPercentage = totalProgressPossible > 0 ? (progressCount / totalProgressPossible) * 100 : 0;

            // Đếm số lượng bài học theo từng khóa học
            var lessonCountsByCourse = _context.Lessons
           .Where(l => l.CourseId != null) // Loại bỏ các bản ghi có CourseId null
           .GroupBy(l => l.CourseId)
           .Select(g => new { CourseId = g.Key, LessonCount = g.Count() })
           .ToDictionary(x => x.CourseId!.ToString(), x => x.LessonCount); // Sử dụng ! để khẳng định không null
                                                                           // Lấy danh sách tên khóa học và số lượng bài học
            var courses = _context.Courses.ToList();
            var lessons = _context.Lessons.ToList();
            var lessonData = courses.Select(c => new
            {
                CourseName = c.Title,
                LessonCount = lessonCountsByCourse.ContainsKey(c.CourseId.ToString())
                                ? lessonCountsByCourse[c.CourseId.ToString()]
                                : 0
            }).ToList();
            // Lấy danh sách tiến độ gần đây
            var progresses = _context.Progresses
                .Where(p => p.IsCompleted)
                .Join(_context.Users,
                    p => p.UserId,
                    u => u.UserId,
                    (p, u) => new { Progress = p, User = u })
                .Join(_context.Lessons,
                    pu => pu.Progress.LessonId,
                    l => l.LessonId,
                    (pu, l) => new { pu.Progress, pu.User, Lesson = l })
                .ToList(); // Chuyển sang LINQ-to-Objects
            // Lấy 5 tiến độ gần đây nhất, sắp xếp theo điểm và ngày
            var latestProgress = progresses
                  .Select(pu => new
                  {
                      UserName = pu.User.Username ?? "Unknown User",
                      LessonName = lessons.FirstOrDefault(l => l.LessonId == pu.Lesson.LessonId)?.Title ?? "Unknown Lesson",
                      CourseName = courses.FirstOrDefault(c => c.CourseId == pu.Lesson.CourseId)?.Title ?? "Unknown Course",
                      Score = pu.Progress.Score ?? 0,
                      Date = pu.Progress.CompletedAt ?? DateTime.Now
                  })
                  .OrderByDescending(x => x.Score) // Sắp xếp theo điểm giảm dần
                  .ThenByDescending(x => x.Date) // Sắp xếp theo ngày nếu điểm bằng nhau
                  .Take(5) // Lấy top 5
                  .ToList();

            var latestCourses = courses
                .Select(c => new
                {
                    CourseName = c.Title,
                    CreatedAt = c.CreatedAt ?? DateTime.Now
                })
                .OrderByDescending(c => c.CreatedAt) // Sắp xếp theo ngày tạo giảm dần
                .Take(5)
                .ToList();
            // Gán dữ liệu vào ViewBag
            ViewBag.UserCount = userCount;
            ViewBag.CourseCount = courseCount;
            ViewBag.LessonCount = lessonCount;
            ViewBag.ProgressCount = progressCount;
            ViewBag.ProgressPercentage = (int)Math.Round(progressPercentage, 0);
            ViewBag.LessonData = lessonData; // Truyền dữ liệu bài học theo khóa học
            ViewBag.LatestProgress = latestProgress; // Truyền dữ liệu tiến độ gần đây
            ViewBag.LatestCourses = latestCourses; // Truyền dữ liệu khóa học mới nhất

            return View();
        }
    }
}
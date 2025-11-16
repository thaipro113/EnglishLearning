using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EnglishLearning.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EnglishLearningDbContext _context;

        public HomeController(ILogger<HomeController> logger, EnglishLearningDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);

                var courses = _context.Courses
                    .Include(c => c.SubLevel)
                    .ToList();

                var vm = new HomeViewModel
                {
                    FullName = user.FullName,
                    Courses = courses
                };

                return View(vm);
            }

            //// Nếu chưa đăng nhập, hiển thị danh sách các khóa học mẫu, hoặc landing page
            //var publicCourses = _context.Courses
            //    .Where(c => c.IsPublic == true) // bạn có thể thêm cột IsPublic để phân biệt
            //    .Take(5) // giới hạn vài khóa học
            //    .ToList();

            var publicVm = new HomeViewModel
            {
                FullName = null,
                LevelName = "Guest",
                //Courses = publicCourses
            };

            return View(publicVm);
        }

    }
}

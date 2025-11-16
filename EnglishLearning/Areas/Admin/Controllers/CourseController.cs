using Microsoft.AspNetCore.Mvc;
using EnglishLearning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnglishLearning.Controllers.Admin
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public CourseController(EnglishLearningDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.SubLevel)
                .ThenInclude(sl => sl.Level)
                .ToListAsync();
            if (courses == null)
            {
                return NotFound("Không tìm thấy khóa học nào.");
            }
            ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
            return View(courses);
        }

        public IActionResult Create()
        {
            ViewBag.SubLevels = _context.SubLevels
                .Include(sl => sl.Level)
                .Select(sl => new SelectListItem
                {
                    Value = sl.SubLevelId.ToString(),
                    Text = $"{sl.SubLevelName} ({sl.Level.LevelName})"
                })
                .ToList();

            if (ViewBag.SubLevels == null || ViewBag.SubLevels.Count == 0)
            {
                ModelState.AddModelError("", "Không có cấp độ con nào để chọn. Vui lòng thêm cấp độ trước.");
            }

            ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SubLevels = _context.SubLevels
                    .Include(sl => sl.Level)
                    .Select(sl => new SelectListItem
                    {
                        Value = sl.SubLevelId.ToString(),
                        Text = $"{sl.SubLevelName} ({sl.Level.LevelName})"
                    })
                    .ToList();
                ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
                return View(course);
            }

            if (_context.Courses.Any(c => c.CourseId == course.CourseId))
            {
                ModelState.AddModelError("CourseId", "Mã khóa học đã tồn tại. Vui lòng chọn mã khác.");
                ViewBag.SubLevels = _context.SubLevels.Include(sl => sl.Level)
                    .Select(sl => new SelectListItem
                    {
                        Value = sl.SubLevelId.ToString(),
                        Text = $"{sl.SubLevelName} ({sl.Level.LevelName})"
                    })
                    .ToList();
                ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
                return View(course);
            }

            course.CreatedAt = DateTime.Now;
            _context.Courses.Add(course);
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm khóa học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.InnerException?.Message);
                ViewBag.SubLevels = _context.SubLevels.Include(sl => sl.Level)
                    .Select(sl => new SelectListItem
                    {
                        Value = sl.SubLevelId.ToString(),
                        Text = $"{sl.SubLevelName} ({sl.Level.LevelName})"
                    })
                    .ToList();
                ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
                return View(course);
            }
        }

        public IActionResult Edit(string id)
        {
            var course = _context.Courses.Find(id);
            if (course == null)
            {
                return NotFound("Không tìm thấy khóa học.");
            }

            ViewBag.SubLevels = _context.SubLevels.Include(sl => sl.Level)
                .Select(sl => new SelectListItem
                {
                    Value = sl.SubLevelId.ToString(),
                    Text = $"{sl.SubLevelName} ({sl.Level.LevelName})",
                    Selected = sl.SubLevelId == course.SubLevelId
                })
                .ToList();

            if (ViewBag.SubLevels == null || ViewBag.SubLevels.Count == 0)
            {
                ModelState.AddModelError("", "Không có cấp độ con nào để chọn. Vui lòng thêm cấp độ trước.");
            }

            ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Course course)
        {
            if (id != course.CourseId)
            {
                return BadRequest("ID không khớp.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.SubLevels = _context.SubLevels.Include(sl => sl.Level)
                    .Select(sl => new SelectListItem
                    {
                        Value = sl.SubLevelId.ToString(),
                        Text = $"{sl.SubLevelName} ({sl.Level.LevelName})"
                    })
                    .ToList();
                ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
                return View(course);
            }

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null)
            {
                return NotFound("Không tìm thấy khóa học.");
            }

            // Giữ nguyên CreatedAt từ bản ghi hiện tại
            course.CreatedAt = existingCourse.CreatedAt;

            // Cập nhật các trường khác
            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;
            existingCourse.SubLevelId = course.SubLevelId;

            try
            {
                _context.Update(existingCourse);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật khóa học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật dữ liệu: " + ex.InnerException?.Message);
                ViewBag.SubLevels = _context.SubLevels.Include(sl => sl.Level)
                    .Select(sl => new SelectListItem
                    {
                        Value = sl.SubLevelId.ToString(),
                        Text = $"{sl.SubLevelName} ({sl.Level.LevelName})"
                    })
                    .ToList();
                ViewBag.LastUpdated = DateTime.Now.ToString("hh:mm tt dd/MM/yyyy (zzz)");
                return View(course);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound("Không tìm thấy khóa học.");
            }
            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa khóa học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa khóa học: " + ex.InnerException?.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
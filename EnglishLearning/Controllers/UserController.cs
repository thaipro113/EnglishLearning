using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using BCrypt.Net;

namespace EnglishLearning.Controllers
{
    public class UserController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public UserController(EnglishLearningDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId}");

            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/User/Profile" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            Debug.WriteLine($"User found: {user?.UserId}, Username: {user?.Username}, Role: {user?.Role}");

            if (user == null)
            {
                TempData["ErrorMessage"] = GetErrorMessage(user?.Role);
                return NotFound();
            }

            // Đồng bộ ImageUrl vào session
            HttpContext.Session.SetString("ImageUrl", user.ImageUrl ?? string.Empty);
            HttpContext.Session.SetString("Username", user.Username ?? string.Empty); // Đảm bảo username cũng đồng bộ

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(User user, IFormFile ImageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null) return NotFound();

            // Cập nhật các trường khác, giữ nguyên Role
            existingUser.FullName = user.FullName;
            existingUser.Level = user.Level;
            existingUser.Purpose = user.Purpose;
            existingUser.PhoneNumber = user.PhoneNumber;
            // Không gán existingUser.Role = user.Role để giữ nguyên Role hiện tại

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }

            // Xử lý ảnh
            // Xử lý ảnh
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Xóa ảnh cũ nếu có và KHÔNG phải là img-default.jpg
                if (!string.IsNullOrEmpty(existingUser.ImageUrl) && existingUser.ImageUrl != "img-default.jpg")
                {
                    var oldFilePath = Path.Combine(uploadsFolder, existingUser.ImageUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existingUser.ImageUrl = fileName;
            }
            else
            {
                // Không upload thì giữ nguyên ảnh cũ (kể cả img-default.jpg)
                existingUser.ImageUrl = existingUser.ImageUrl ?? "img-default.jpg";
            }


            try
            {
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

                // Đồng bộ ImageUrl và Username vào session ngay sau khi lưu
                HttpContext.Session.SetString("ImageUrl", existingUser.ImageUrl ?? string.Empty);
                HttpContext.Session.SetString("Username", existingUser.Username ?? string.Empty);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // basic validation
            if (model == null)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                TempData["ShowPasswordForm"] = "true";
                return RedirectToAction("EditProfile");
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập mật khẩu mới và xác nhận.";
                TempData["ShowPasswordForm"] = "true";
                return RedirectToAction("EditProfile");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "❌ Mật khẩu mới và xác nhận không khớp.";
                TempData["ShowPasswordForm"] = "true";
                return RedirectToAction("EditProfile");
            }

            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("EditProfile");
            }

            var existingHash = existingUser.PasswordHash ?? string.Empty;
            bool hasBcryptHash = existingHash.StartsWith("$2a$") || existingHash.StartsWith("$2b$") || existingHash.StartsWith("$2y$");

            if (hasBcryptHash)
            {
                if (string.IsNullOrWhiteSpace(model.OldPassword))
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập mật khẩu cũ.";
                    TempData["ShowPasswordForm"] = "true";
                    return RedirectToAction("EditProfile");
                }

                bool verifyOk;
                try
                {
                    verifyOk = BCrypt.Net.BCrypt.Verify(model.OldPassword, existingHash);
                }
                catch
                {
                    verifyOk = false;
                }

                if (!verifyOk)
                {
                    TempData["ErrorMessage"] = "❌ Mật khẩu cũ không đúng.";
                    TempData["ShowPasswordForm"] = "true";
                    return RedirectToAction("EditProfile");
                }
            }
            else
            {
                // External account: hiện tại cho phép đặt mật khẩu mới trực tiếp.
                // (Trong production có thể yêu cầu verify email/OTP trước khi cho phép.)
            }

            // Hash và lưu mật khẩu mới
            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            try
            {
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Đổi mật khẩu thành công!";
                TempData["ShowPasswordForm"] = "false";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi đổi mật khẩu: {ex.Message}";
                TempData["ShowPasswordForm"] = "true";
            }

            return RedirectToAction("EditProfile"); // hoặc "Profile" tùy bạn muốn UX
        }

        public async Task<IActionResult> Progress()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            Debug.WriteLine($"UserId from session: {userId}");

            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/User/Progress" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = GetErrorMessage(user?.Role);
                return NotFound();
            }

            // Lấy tiến độ của người dùng
            var userProgress = await _context.Progresses
                .Where(p => p.UserId == userId && p.IsCompleted)
                .Join(_context.Lessons,
                    p => p.LessonId,
                    l => l.LessonId,
                    (p, l) => new { Progress = p, Lesson = l })
                .Join(_context.Courses,
                    pl => pl.Lesson.CourseId,
                    c => c.CourseId,
                    (pl, c) => new
                    {
                        CourseId = c.CourseId,
                        CourseName = c.Title,
                        LessonName = pl.Lesson.Title,
                        Score = pl.Progress.Score ?? 0,
                        CompletedAt = pl.Progress.CompletedAt
                    })
                .ToListAsync();

            var progressSummary = userProgress
                .GroupBy(p => p.CourseName)
                .Select(g => new
                {
                    CourseId = g.First().CourseId,
                    CourseName = g.Key,
                    TotalLessons = g.Count(),
                    AverageScore = Math.Round(g.Average(x => x.Score), 2),
                    LastCompleted = g.Max(x => x.CompletedAt)
                })
                .ToList();

            ViewBag.UserProgress = progressSummary;

            return View();
        }

        public async Task<IActionResult> ProgressDetail(string courseId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/User/ProgressDetail" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = GetErrorMessage(user?.Role);
                return NotFound();
            }

            // Lấy chi tiết tiến độ cho khóa học cụ thể
            var progressDetails = await _context.Progresses
                .Where(p => p.UserId == userId && p.IsCompleted)
                .Join(_context.Lessons,
                    p => p.LessonId,
                    l => l.LessonId,
                    (p, l) => new { Progress = p, Lesson = l })
                .Join(_context.Courses,
                    pl => pl.Lesson.CourseId,
                    c => c.CourseId,
                    (pl, c) => new
                    {
                        CourseId = c.CourseId,
                        CourseName = c.Title,
                        LessonName = pl.Lesson.Title,
                        Score = pl.Progress.Score ?? 0,
                        CompletedAt = pl.Progress.CompletedAt
                    })
                .Where(p => p.CourseId == courseId)
                .OrderByDescending(p => p.CompletedAt)
                .ToListAsync();

            if (progressDetails == null || !progressDetails.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy tiến độ cho khóa học này.";
                return RedirectToAction("Progress");
            }

            ViewBag.CourseName = progressDetails.First().CourseName;
            ViewBag.ProgressDetails = progressDetails;

            return View();
        }

        private string GetErrorMessage(string? role, string? detailedMessage = null)
        {
            if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(detailedMessage))
                return $"Lỗi chi tiết: {detailedMessage}";
            return "Có lỗi xảy ra. Vui lòng thử lại sau.";
        }
    }
}
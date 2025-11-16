using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Text.Json;

namespace EnglishLearning.Controllers
{
    public class AccountController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public AccountController(EnglishLearningDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse")
            };
            return Challenge(properties, "Facebook");
        }
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            // Tìm hoặc tạo người dùng
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Username = email,
                    Email = email,
                    FullName = name ?? "Facebook User",
                    CreatedAt = DateTime.Now,
                    Role = "User",
                    ImageUrl = "img-default.jpg",
                    PhoneNumber = "1234567890", // Bạn có thể lấy thêm từ Facebook nếu cần
                    PasswordHash = ""
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra bước
            bool hasCompletedSteps = !string.IsNullOrEmpty(user.Level) &&
                                     !string.IsNullOrEmpty(user.Occupation) &&
                                     !string.IsNullOrEmpty(user.Purpose);

            // Lưu session
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");
            HttpContext.Session.SetString("ImageUrl", user.ImageUrl ?? "");

            // Điều hướng dựa trên trạng thái hoàn thành
            if (hasCompletedSteps)
            {
                return RedirectToAction("Index", "Course");
            }
            else
            {
                // Chỉ lưu TempUserId nếu chưa hoàn thành để sử dụng trong các bước
                HttpContext.Session.SetInt32("TempUserId", user.UserId);
                return RedirectToAction("Step1");
            }
        }

        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            // Tìm user theo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // Nếu chưa có thì tạo user mới
                user = new User
                {
                    Username = email, // hoặc tạo một username riêng nếu muốn
                    Email = email,
                    FullName = name ?? "Google User",
                    CreatedAt = DateTime.Now,
                    Role = "User",
                    ImageUrl = "img-default.jpg",
                    PhoneNumber = "1234567890",
                    PasswordHash = ""
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra xem user đã hoàn thành 3 bước chưa
            bool hasCompletedSteps = !string.IsNullOrEmpty(user.Level) &&
                                    !string.IsNullOrEmpty(user.Occupation) &&
                                    !string.IsNullOrEmpty(user.Purpose);

            // Lưu thông tin vào session
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");
            HttpContext.Session.SetString("ImageUrl", user.ImageUrl ?? "");

            // Điều hướng dựa trên trạng thái hoàn thành
            if (hasCompletedSteps)
            {
                return RedirectToAction("Index", "Course");
            }
            else
            {
                // Chỉ lưu TempUserId nếu chưa hoàn thành để sử dụng trong các bước
                HttpContext.Session.SetInt32("TempUserId", user.UserId);
                return RedirectToAction("Step1");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ✅ Chuẩn hóa input: xóa khoảng trắng đầu/cuối
            model.Email = model.Email?.Trim();
            model.Username = model.Username?.Trim();
            model.FullName = model.FullName?.Trim();
            model.PhoneNumber = model.PhoneNumber?.Trim();

            // Kiểm tra trùng username
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            // Kiểm tra trùng email
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(model);
            }

            // Kiểm tra trùng số điện thoại
            if (_context.Users.Any(u => u.PhoneNumber == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại.");
                return View(model);
            }

            // Tạo user mới
            var user = new User
            {
                Username = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.Now,
                Role = "User",
                ImageUrl = "img-default.jpg"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("TempUserId", user.UserId);

            return RedirectToAction("Step1");
        }


        [HttpGet]
        public IActionResult Step1()
        {
            // Danh sách hiển thị bằng tiếng Việt, ánh xạ với tiếng Anh
            ViewBag.Levels = new Dictionary<string, string>
            {
                {"Beginner", "Mới bắt đầu (Tôi không biết gì về tiếng anh / tôi có biết một ít)"},
                {"Intermediate", "Trung cấp (Tôi đã biết rõ về các kiến thức cơ bản)"},
                {"Advanced", "Nâng cao (Tôi đã thành thạo về các kiến thức trong tiếng anh)"}
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Step1(string level)
        {
            if (string.IsNullOrEmpty(level))
            {
                ViewBag.Error = "Vui lòng chọn cấp độ.";
                ViewBag.Levels = new Dictionary<string, string>
                {
                    {"Beginner", "Mới bắt đầu"},
                    {"Intermediate", "Trung cấp"},
                    {"Advanced", "Nâng cao"}
                };
                return View();
            }

            var userId = HttpContext.Session.GetInt32("TempUserId");
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Level = level; // Lưu giá trị tiếng Anh
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Step2");
        }

        [HttpGet]
        public IActionResult Step2()
        {
            // Danh sách hiển thị bằng tiếng Việt cho vai trò học tập
            ViewBag.Occupations = new List<string>
            {
                "Học sinh", "Sinh viên cao đẳng", "Sinh viên đại học", "Nhân viên",
                "Tự học", "Giáo viên", "Chủ doanh nghiệp", "Người về hưu"
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Step2(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                ViewBag.Error = "Vui lòng chọn vai trò học tập.";
                ViewBag.Occupations = new List<string>
                {
                    "Học sinh", "Sinh viên cao đẳng", "Sinh viên đại học", "Nhân viên",
                    "Tự học", "Giáo viên", "Chủ doanh nghiệp", "Người về hưu"
                };
                return View();
            }

            var userId = HttpContext.Session.GetInt32("TempUserId");
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Occupation = role; // Lưu vai trò học tập vào Occupation
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Step3");
        }

        [HttpGet]
        public IActionResult Step3()
        {
            // Danh sách hiển thị bằng tiếng Việt
            ViewBag.Purposes = new List<string>
            {
                "Giao tiếp", "Công việc", "Học tập", "Du lịch", "Ôn thi (TOEIC/IELTS/VSTEP)",
                "Thăng tiến sự nghiệp", "Sở thích", "Định cư", "Trao đổi văn hóa"
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Step3(string purpose)
        {
            if (string.IsNullOrEmpty(purpose))
            {
                ViewBag.Error = "Vui lòng chọn mục đích.";
                ViewBag.Purposes = new List<string>
        {
            "Giao tiếp", "Công việc", "Học tập", "Du lịch", "Ôn thi (TOEIC/IELTS/VSTEP)",
            "Thăng tiến sự nghiệp", "Sở thích", "Định cư", "Trao đổi văn hóa"
        };
                return View();
            }

            var userId = HttpContext.Session.GetInt32("TempUserId");
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Purpose = purpose; // Đảm bảo lưu
                    await _context.SaveChangesAsync();

                    // Debug: Kiểm tra giá trị sau khi lưu
                    Console.WriteLine($"Saved Purpose for UserId {userId}: {user.Purpose}");

                    // Chuyển từ TempUserId sang UserId và xóa session tạm
                    HttpContext.Session.SetString("FullName", user.FullName);
                    HttpContext.Session.SetInt32("UserId", userId.Value);
                    HttpContext.Session.Remove("TempUserId");
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);
                }
            }

            return RedirectToAction("Index", "Course");
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            // Nếu đã đăng nhập thì chuyển hướng về trang khóa học
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Course");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Kiểm tra đã hoàn thành 3 bước chưa
                bool hasCompletedSteps = !string.IsNullOrEmpty(user.Level) &&
                                         !string.IsNullOrEmpty(user.Occupation) &&
                                         !string.IsNullOrEmpty(user.Purpose);

                if (hasCompletedSteps)
                {
                    // Đăng nhập đầy đủ
                    HttpContext.Session.SetString("FullName", user.FullName);
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role ?? "");
                    HttpContext.Session.SetString("ImageUrl", user.ImageUrl ?? "");

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Course");
                    }
                }
                else
                {
                    // Chưa hoàn thành các bước -> buộc sang Step1
                    HttpContext.Session.SetInt32("TempUserId", user.UserId);
                    return RedirectToAction("Step1");
                }
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        // ✅ Rất quan trọng: Bạn cũng cần có phương thức GET Login để lấy returnUrl từ URL
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login");

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
          
            return RedirectToAction("Login", "Account");
        }
    }
}
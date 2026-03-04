using EnglishLearning.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnglishLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public AuthController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Nếu đã đăng nhập admin rồi → chuyển thẳng vào dashboard
            if (User.Identity?.IsAuthenticated == true &&
                User.HasClaim(c => c.Type == "AuthScheme" && c.Value == "AdminScheme"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Chỉ cho phép Role = Admin vào khu vực này
                if (user.Role != "Admin")
                {
                    ViewBag.Error = "Bạn không có quyền truy cập trang quản trị.";
                    ViewBag.ReturnUrl = returnUrl;
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("FullName", user.FullName ?? user.Username),
                    new Claim("ImageUrl", user.ImageUrl ?? "img-default.jpg"),
                    new Claim("AuthScheme", "AdminScheme") // Đánh dấu đây là admin login
                };

                var claimsIdentity = new ClaimsIdentity(claims, "AdminScheme");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync("AdminScheme", new ClaimsPrincipal(claimsIdentity), authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
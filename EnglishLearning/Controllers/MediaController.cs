using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace EnglishLearning.Controllers
{
    [Route("uploads")]
    public class MediaController : Controller
    {
        [HttpGet("{*path}")]
        public IActionResult GetMedia(string path)
        {
            // Bỏ thẻ [Authorize] vì website dùng Session("UserId") để đăng nhập thay vì Claims chuẩn của .NET.
            // Cần tự check bằng Session.
            var userId = HttpContext.Session.GetInt32("UserId");
            var isUserAuthenticatedByCookie = HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated;

            // Nếu không có Session("UserId") VÀ không có Cookie Auth -> Chặn
            if (userId == null && !isUserAuthenticatedByCookie)
            {
                return Unauthorized(); // Trả về 401 nếu chưa đăng nhập (đỡ bị redirect sang trang HTML ở thẻ <img>)
            }

            if (string.IsNullOrEmpty(path))
            {
                return NotFound();
            }

            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", path);

            // Bảo mật: Ngăn chặn lỗi Directory Traversal
            var fullPath = Path.GetFullPath(physicalPath);
            var storagePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads"));
            if (!fullPath.StartsWith(storagePath))
            {
                return BadRequest();
            }

            if (!System.IO.File.Exists(physicalPath))
            {
                return NotFound();
            }

            // Tự động nhận diện định dạng file (Content-Type)
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(physicalPath, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            // Trả về file vật lý với cờ enableRangeProcessing = true để hỗ trợ tính năng TUA (Seek) Audio/Video
            return PhysicalFile(physicalPath, contentType, enableRangeProcessing: true);
        }
    }
}

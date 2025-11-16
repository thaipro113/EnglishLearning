using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EnglishLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProgressController : Controller
    {
        private readonly EnglishLearningDbContext _context;
        private const int PageSize = 10; // Số bản ghi mỗi trang (có thể thay đổi)

        public ProgressController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            // Đảm bảo page không âm hoặc vượt quá
            page = page < 1 ? 1 : page;

            // Lấy dữ liệu với Include
            var query = _context.Progresses
                .Include(p => p.User)
                .Include(p => p.Lesson)
                .AsQueryable();

            // Đếm tổng số bản ghi
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Giới hạn page không vượt quá totalPages
            if (page > totalPages && totalPages > 0)
                page = totalPages;

            // Lấy dữ liệu cho trang hiện tại
            var progressList = await query
                .OrderByDescending(p => p.CompletedAt) // Sắp xếp theo thời gian hoàn thành (mới nhất trước)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Truyền thông tin phân trang qua ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return View(progressList);
        }
    }
}
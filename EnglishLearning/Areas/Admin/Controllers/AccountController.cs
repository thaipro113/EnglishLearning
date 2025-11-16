using Microsoft.AspNetCore.Mvc;
using EnglishLearning.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EnglishLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public UserController(EnglishLearningDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId)
            {
                Debug.WriteLine($"Mismatch: id={id}, user.UserId={user.UserId}");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        Debug.WriteLine($"User not found for id: {id}");
                        return NotFound();
                    }

                    // Cập nhật tất cả các trường
                    existingUser.Username = user.Username;
                    existingUser.PasswordHash = user.PasswordHash;
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Role = user.Role;
                    existingUser.Occupation = user.Occupation;
                    existingUser.Level = user.Level;
                    existingUser.Purpose = user.Purpose; // Đảm bảo Purpose được cập nhật
                    existingUser.ImageUrl = user.ImageUrl;

                    _context.Users.Update(existingUser);
                    await _context.SaveChangesAsync();
                    Debug.WriteLine($"User updated successfully: {user.UserId}, Purpose: {user.Purpose}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Debug.WriteLine($"Concurrency error: {ex.Message}");
                    if (!_context.Users.Any(u => u.UserId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating user: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật người dùng.");
                }
            }
            else
            {
                Debug.WriteLine("ModelState is invalid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                Debug.WriteLine($"User not found for id: {id}");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Xóa tất cả Paragraphs liên quan đến người dùng
                var paragraphs = await _context.Paragraphs
                    .Where(p => p.UserId == id)
                    .ToListAsync();
                if (paragraphs.Any())
                {
                    _context.Paragraphs.RemoveRange(paragraphs);
                    Debug.WriteLine($"Removed {paragraphs.Count} Paragraphs for UserId: {id}");
                }

                // Xóa tất cả TranslationHistory liên quan đến các Paragraphs đã xóa
                var paragraphIds = paragraphs.Select(p => p.Id).ToList();
                if (paragraphIds.Any())
                {
                    var histories = await _context.TranslationHistories
                        .Where(h => paragraphIds.Contains(h.ParagraphId))
                        .ToListAsync();
                    if (histories.Any())
                    {
                        _context.TranslationHistories.RemoveRange(histories);
                        Debug.WriteLine($"Removed {histories.Count} TranslationHistory records");
                    }
                }

                // Xóa người dùng
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                Debug.WriteLine($"Deleted User ID: {id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting user: {ex.Message}");
                // Có thể thêm thông báo lỗi cho admin nếu cần
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
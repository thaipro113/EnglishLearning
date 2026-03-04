using EnglishLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public FlashcardController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sets = await _context.FlashcardSets
                .Include(f => f.Flashcards)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return View(sets);
        }

        public async Task<IActionResult> Study(int? id)
        {
            if (id == null) return NotFound();

            var set = await _context.FlashcardSets
                .Include(f => f.Flashcards)
                .FirstOrDefaultAsync(m => m.FlashcardSetId == id);

            if (set == null) return NotFound();

            return View(set);
        }
    }
}

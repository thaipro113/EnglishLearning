using EnglishLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishLearning.Areas.Admin.Attributes;

namespace EnglishLearning.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class FlashcardController : Controller
    {
        private readonly EnglishLearningDbContext _context;

        public FlashcardController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sets = await _context.FlashcardSets.Include(f => f.Flashcards).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return View(sets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlashcardSet flashcardSet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flashcardSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flashcardSet);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var set = await _context.FlashcardSets.FindAsync(id);
            if (set == null) return NotFound();

            return View(set);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlashcardSet flashcardSet)
        {
            if (id != flashcardSet.FlashcardSetId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flashcardSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlashcardSetExists(flashcardSet.FlashcardSetId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(flashcardSet);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var set = await _context.FlashcardSets
                .FirstOrDefaultAsync(m => m.FlashcardSetId == id);
            if (set == null) return NotFound();

            return View(set);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var set = await _context.FlashcardSets.FindAsync(id);
            if (set != null)
            {
                _context.FlashcardSets.Remove(set);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Manage Cards in a Set
        public async Task<IActionResult> ManageCards(int id)
        {
            var set = await _context.FlashcardSets.Include(f => f.Flashcards).FirstOrDefaultAsync(f => f.FlashcardSetId == id);
            if (set == null) return NotFound();
            
            ViewBag.FlashcardSetId = id;
            ViewBag.SetTitle = set.Title;
            return View(set.Flashcards.ToList());
        }

        public IActionResult CreateCard(int setId)
        {
            ViewBag.FlashcardSetId = setId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(Flashcard flashcard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flashcard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageCards), new { id = flashcard.FlashcardSetId });
            }
            ViewBag.FlashcardSetId = flashcard.FlashcardSetId;
            return View(flashcard);
        }
        
        public async Task<IActionResult> EditCard(int? id)
        {
            if (id == null) return NotFound();

            var card = await _context.Flashcards.FindAsync(id);
            if (card == null) return NotFound();

            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCard(int id, Flashcard card)
        {
            if (id != card.FlashcardId) return NotFound();

            if (ModelState.IsValid)
            {
                 _context.Update(card);
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(ManageCards), new { id = card.FlashcardSetId });
            }
            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Flashcards.FindAsync(id);
            if (card != null)
            {
                int setId = card.FlashcardSetId;
                _context.Flashcards.Remove(card);
                await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(ManageCards), new { id = setId });
            }
            return RedirectToAction(nameof(Index));
        }


        private bool FlashcardSetExists(int id)
        {
            return _context.FlashcardSets.Any(e => e.FlashcardSetId == id);
        }
    }
}

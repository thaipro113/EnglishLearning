using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Controllers.Admin
{
    [Area("Admin")]
    public class LessonController : Controller
    {
        private readonly EnglishLearningDbContext _context;
        private const int PageSize = 10;
        public LessonController(EnglishLearningDbContext context)
        {
            _context = context;
        }

        // GET: Index (Hỗ trợ tìm kiếm theo Title)
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            page = page < 1 ? 1 : page;

            var lessonsQuery = _context.Lessons
                .Include(l => l.Course)
                .Include(l => l.Quizzes)
                .AsQueryable();

            // === TÌM KIẾM ===
            if (!string.IsNullOrEmpty(searchString))
            {
                var searchLower = searchString.ToLower();
                lessonsQuery = lessonsQuery.Where(l =>
                    l.Title.ToLower().Contains(searchLower) ||
                    l.LessonId.ToLower().Contains(searchLower));
            }

            // === ĐẾM TỔNG ===
            var totalItems = await lessonsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            if (page > totalPages && totalPages > 0)
                page = totalPages;

            // === LẤY DỮ LIỆU CHO TRANG HIỆN TẠI ===
            var lessons = await lessonsQuery
                .OrderBy(l => l.CourseId)
                .ThenBy(l => l.OrderIndex) // Sắp xếp theo khóa học → thứ tự
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // === TRUYỀN DỮ LIỆU PHÂN TRANG ===
            ViewBag.SearchString = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(lessons);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title");
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lesson lesson, List<Quiz> quizzes)
        {
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", lesson.CourseId);

            if (ModelState.IsValid)
            {
                // Gán LessonType là "Normal" cho bài học thường
                lesson.LessonType = "Normal";
                lesson.CreatedAt = DateTime.Now;
                _context.Add(lesson);
                await _context.SaveChangesAsync();

                if (quizzes != null && quizzes.Any())
                {
                    Console.WriteLine($"[DEBUG] Total questions from form: {quizzes.Count}");
                    foreach (var quiz in quizzes)
                    {
                        quiz.LessonId = lesson.LessonId;
                        var existingQuiz = await _context.Quizzes
                            .FirstOrDefaultAsync(q => q.LessonId == lesson.LessonId &&
                                               q.Question == quiz.Question &&
                                               q.CorrectAnswer == quiz.CorrectAnswer.ToUpper() &&
                                               q.OptionA == quiz.OptionA &&
                                               q.OptionB == quiz.OptionB &&
                                               q.OptionC == quiz.OptionC &&
                                               q.OptionD == quiz.OptionD);
                        if (existingQuiz == null)
                        {
                            _context.Quizzes.Add(quiz);
                            Console.WriteLine($"[DEBUG] Add new QuizId: {quiz.QuizId}");
                        }
                        else
                        {
                            Console.WriteLine($"[DEBUG] Skip QuizId: {quiz.QuizId} due to duplication with DB");
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["success"] = " Thêm bài học thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            TempData["error"] = " Lỗi khi thêm bài học!";
            return View(lesson);
        }

        // GET: CreateToeic
        public IActionResult CreateToeic()
        {
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title");
            return View();
        }

        // POST: CreateToeic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateToeic(Lesson lesson, List<Quiz> quizzes, IFormFile? AudioFile)
        {
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", lesson.CourseId);

            if (ModelState.IsValid)
            {
                // Gán LessonType là "TOEIC"
                lesson.LessonType = "TOEIC";
                lesson.CreatedAt = DateTime.Now;
                // Xử lý file audio nếu có
                if (AudioFile != null && AudioFile.Length > 0)
                {
                    var audioDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/audio");
                    if (!Directory.Exists(audioDir))
                        Directory.CreateDirectory(audioDir);

                    var audioName = Guid.NewGuid() + Path.GetExtension(AudioFile.FileName);
                    var audioPath = Path.Combine(audioDir, audioName);

                    using (var stream = new FileStream(audioPath, FileMode.Create))
                    {
                        await AudioFile.CopyToAsync(stream);
                    }

                    lesson.AudioPath = "/uploads/audio/" + audioName;
                }

                // Lưu bài học trước để có LessonId
                _context.Add(lesson);
                await _context.SaveChangesAsync();

                // Lưu quiz nếu có
                if (quizzes != null && quizzes.Any())
                {
                    foreach (var quiz in quizzes)
                    {
                        quiz.LessonId = lesson.LessonId;

                        // Nếu có ảnh thì lưu vào wwwroot/uploads/images
                        if (quiz.ImageFile != null && quiz.ImageFile.Length > 0)
                        {
                            var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                            if (!Directory.Exists(imgDir))
                                Directory.CreateDirectory(imgDir);

                            var imgName = Guid.NewGuid() + Path.GetExtension(quiz.ImageFile.FileName);
                            var imgPath = Path.Combine(imgDir, imgName);

                            using (var stream = new FileStream(imgPath, FileMode.Create))
                            {
                                await quiz.ImageFile.CopyToAsync(stream);
                            }

                            quiz.ImagePath = "/uploads/images/" + imgName;
                        }

                        // 🔎 Tìm quiz trùng trong DB
                        var existingQuiz = await _context.Quizzes.FirstOrDefaultAsync(q =>
                            q.LessonId == quiz.LessonId &&
                            q.Question == quiz.Question &&
                            q.OptionA == quiz.OptionA &&
                            q.OptionB == quiz.OptionB &&
                            q.OptionC == quiz.OptionC &&
                            q.OptionD == quiz.OptionD &&
                            q.CorrectAnswer == quiz.CorrectAnswer
                        );

                        if (existingQuiz == null)
                        {
                            // Không trùng -> thêm mới
                            _context.Quizzes.Add(quiz);
                            Console.WriteLine($"[DEBUG] Add new Quiz: {quiz.Question}");
                        }
                        else
                        {
                            // Trùng câu hỏi nhưng nếu có ảnh mới thì update
                            if (!string.IsNullOrEmpty(quiz.ImagePath))
                            {
                                existingQuiz.ImagePath = quiz.ImagePath;
                                _context.Quizzes.Update(existingQuiz);
                                Console.WriteLine($"[DEBUG] Update image for Quiz: {quiz.Question}");
                            }
                            else
                            {
                                Console.WriteLine($"[DEBUG] Skip duplicate Quiz without new image: {quiz.Question}");
                            }
                        }

                    }


                    await _context.SaveChangesAsync();
                }

                TempData["success"] = "Thêm đề thi TOEIC thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Log lỗi ModelState (nếu có)
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            TempData["error"] = "Lỗi khi thêm đề thi TOEIC!";
            return View(lesson);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var lesson = await _context.Lessons
              .Include(l => l.Quizzes)
              .FirstOrDefaultAsync(l => l.LessonId == id);

            if (lesson == null) return NotFound();

            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", lesson.CourseId);
            return View(lesson);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Lesson lesson, List<Quiz> quizzes, IFormFile? AudioFile)
        {
            if (id != lesson.LessonId)
            {
                return BadRequest("ID không khớp.");
            }

            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", lesson.CourseId);

            if (!ModelState.IsValid)
            {
                TempData["error"] = " Dữ liệu không hợp lệ!";
                return View(lesson);
            }

            var existingLesson = await _context.Lessons
                .Include(l => l.Quizzes)
                .FirstOrDefaultAsync(l => l.LessonId == id);

            if (existingLesson == null)
            {
                return NotFound("Bài học không tồn tại.");
            }

            // Giữ nguyên LessonType từ DB nếu không có giá trị mới
            lesson.LessonType = string.IsNullOrEmpty(lesson.LessonType) ? existingLesson.LessonType : lesson.LessonType;

            // Update lesson fields
            existingLesson.Title = lesson.Title;
            existingLesson.Content = lesson.Content;
            existingLesson.CourseId = lesson.CourseId;
            existingLesson.VideoUrl = lesson.VideoUrl;
            existingLesson.OrderIndex = lesson.OrderIndex;
            existingLesson.LessonType = lesson.LessonType;

            // ✅ Update Audio nếu có file mới (TOEIC)
            if (AudioFile != null && AudioFile.Length > 0 && existingLesson.LessonType == "TOEIC")
            {
                // Xóa file audio cũ nếu có
                if (!string.IsNullOrEmpty(existingLesson.AudioPath))
                {
                    var oldAudio = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingLesson.AudioPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldAudio))
                    {
                        System.IO.File.Delete(oldAudio);
                        Console.WriteLine($"[DEBUG] Deleted old audio: {oldAudio}");
                    }
                }

                var audioDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/audio");
                if (!Directory.Exists(audioDir)) Directory.CreateDirectory(audioDir);

                var audioName = Guid.NewGuid() + Path.GetExtension(AudioFile.FileName);
                var audioPath = Path.Combine(audioDir, audioName);

                using (var stream = new FileStream(audioPath, FileMode.Create))
                {
                    await AudioFile.CopyToAsync(stream);
                }

                existingLesson.AudioPath = "/uploads/audio/" + audioName;
            }

            var existingQuizzes = existingLesson.Quizzes.ToList();

            if (quizzes != null && quizzes.Any())
            {
                var processedQuizIds = new HashSet<int>();

                foreach (var quiz in quizzes)
                {
                    if (quiz.IsDeleted)
                    {
                        var quizToDelete = existingQuizzes.FirstOrDefault(q => q.QuizId == quiz.QuizId);
                        if (quizToDelete != null)
                        {
                            // Xóa ảnh nếu có
                            if (!string.IsNullOrEmpty(quizToDelete.ImagePath))
                            {
                                var oldImg = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", quizToDelete.ImagePath.TrimStart('/'));
                                if (System.IO.File.Exists(oldImg))
                                {
                                    System.IO.File.Delete(oldImg);
                                }
                            }

                            _context.Quizzes.Remove(quizToDelete);
                        }
                    }
                    else
                    {
                        if (quiz.QuizId > 0)
                        {
                            // Update quiz
                            var existingQuiz = existingQuizzes.FirstOrDefault(q => q.QuizId == quiz.QuizId);
                            if (existingQuiz != null)
                            {
                                existingQuiz.Question = quiz.Question;
                                existingQuiz.OptionA = quiz.OptionA;
                                existingQuiz.OptionB = quiz.OptionB;
                                existingQuiz.OptionC = quiz.OptionC;
                                existingQuiz.OptionD = quiz.OptionD;
                                existingQuiz.CorrectAnswer = quiz.CorrectAnswer;

                                // ✅ Xử lý ảnh mới nếu có
                                if (quiz.ImageFile != null && quiz.ImageFile.Length > 0)
                                {
                                    // Xóa ảnh cũ nếu tồn tại
                                    if (!string.IsNullOrEmpty(existingQuiz.ImagePath))
                                    {
                                        var oldImg = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingQuiz.ImagePath.TrimStart('/'));
                                        if (System.IO.File.Exists(oldImg))
                                        {
                                            System.IO.File.Delete(oldImg);
                                            Console.WriteLine($"[DEBUG] Deleted old image: {oldImg}");
                                        }
                                    }

                                    // Tạo thư mục nếu chưa có
                                    var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                                    if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);

                                    // Lưu file mới
                                    var imgName = Guid.NewGuid() + Path.GetExtension(quiz.ImageFile.FileName);
                                    var imgPath = Path.Combine(imgDir, imgName);

                                    using (var stream = new FileStream(imgPath, FileMode.Create))
                                    {
                                        await quiz.ImageFile.CopyToAsync(stream);
                                    }

                                    existingQuiz.ImagePath = "/uploads/images/" + imgName;
                                }

                                _context.Quizzes.Update(existingQuiz);
                            }
                        }
                        else
                        {
                            // Add new quiz
                            quiz.LessonId = lesson.LessonId;

                            // Nếu có ảnh khi thêm mới
                            if (quiz.ImageFile != null && quiz.ImageFile.Length > 0)
                            {
                                var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                                if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);

                                var imgName = Guid.NewGuid() + Path.GetExtension(quiz.ImageFile.FileName);
                                var imgPath = Path.Combine(imgDir, imgName);

                                using (var stream = new FileStream(imgPath, FileMode.Create))
                                {
                                    await quiz.ImageFile.CopyToAsync(stream);
                                }

                                quiz.ImagePath = "/uploads/images/" + imgName;
                            }

                            _context.Quizzes.Add(quiz);
                        }
                    }

                }

            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = " Cập nhật bài học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = " Lỗi khi cập nhật: " + ex.Message;
                return View(lesson);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Quizzes)
                .FirstOrDefaultAsync(l => l.LessonId == id);

            if (lesson == null)
            {
                TempData["error"] = " Bài học không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            // ✅ Nếu TOEIC → xóa audio, video, images
            if (lesson.LessonType == "TOEIC")
            {
                if (!string.IsNullOrEmpty(lesson.AudioPath))
                {
                    var audioPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lesson.AudioPath.TrimStart('/'));
                    if (System.IO.File.Exists(audioPath))
                        System.IO.File.Delete(audioPath);
                }

                if (!string.IsNullOrEmpty(lesson.VideoUrl))
                {
                    var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", lesson.VideoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(videoPath))
                        System.IO.File.Delete(videoPath);
                }

                foreach (var quiz in lesson.Quizzes)
                {
                    if (!string.IsNullOrEmpty(quiz.ImagePath))
                    {
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", quiz.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(imgPath))
                            System.IO.File.Delete(imgPath);
                    }
                }
            }

            _context.Quizzes.RemoveRange(lesson.Quizzes);
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            TempData["success"] = " Xóa bài học và file đính kèm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                var existingQuiz = await _context.Quizzes
                  .FirstOrDefaultAsync(q => q.LessonId == quiz.LessonId &&
                             q.Question == quiz.Question &&
                             q.CorrectAnswer == quiz.CorrectAnswer &&
                             q.OptionA == quiz.OptionA &&
                             q.OptionB == quiz.OptionB &&
                             q.OptionC == quiz.OptionC &&
                             q.OptionD == quiz.OptionD);
                if (existingQuiz == null)
                {
                    _context.Quizzes.Add(quiz);
                    await _context.SaveChangesAsync();
                    TempData["success"] = " Thêm câu hỏi thành công!";
                }
                else
                {
                    TempData["error"] = " Câu hỏi đã tồn tại!";
                }
            }
            else
            {
                TempData["error"] = " Lỗi khi thêm câu hỏi!";
            }

            return RedirectToAction(nameof(Edit), new { id = quiz.LessonId });
        }

        [HttpPost]
        public async Task<IActionResult> EditQuiz(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();
                TempData["success"] = " Cập nhật câu hỏi thành công!";
            }
            else
            {
                TempData["error"] = " Lỗi khi cập nhật câu hỏi!";
            }

            return RedirectToAction(nameof(Edit), new { id = quiz.LessonId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz != null)
            {
                string lessonId = quiz.LessonId;
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
                TempData["success"] = " Xóa câu hỏi thành công!";
                return RedirectToAction(nameof(Edit), new { id = lessonId });
            }

            TempData["error"] = " Không tìm thấy câu hỏi!";
            return RedirectToAction(nameof(Index));
        }
    }
}
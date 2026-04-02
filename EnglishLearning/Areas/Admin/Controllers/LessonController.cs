using EnglishLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnglishLearning.Areas.Admin.Attributes;

namespace EnglishLearning.Controllers.Admin
{
    [Area("Admin")]
    [AdminAuthorize]
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
                    var audioDir = Path.Combine(Directory.GetCurrentDirectory(), "Storage/uploads/audio");
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
                lesson.Quizzes = new List<Quiz>();
                _context.Add(lesson);
                await _context.SaveChangesAsync();

                // Xử lý hình ảnh nhóm cho Part 6 và Part 7
                var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "Storage/uploads/images");
                if (!Directory.Exists(imgDir))
                    Directory.CreateDirectory(imgDir);

                // Debug: Log tất cả files trong request
                Console.WriteLine($"[DEBUG CREATE] ===== FILES IN REQUEST =====");
                Console.WriteLine($"[DEBUG CREATE] Total files: {Request.Form.Files.Count}");
                foreach (var file in Request.Form.Files)
                {
                    Console.WriteLine($"[DEBUG CREATE] File: Name='{file.Name}', FileName='{file.FileName}', Length={file.Length}");
                }
                Console.WriteLine($"[DEBUG CREATE] =============================");

                var groupImagePaths = new Dictionary<string, string>();

                // Part 6: 4 nhóm (mỗi nhóm 4 câu)
                for (int g = 0; g < 4; g++)
                {
                    var fileKey = $"Part6ImageGroup{g}";
                    var imageFile = Request.Form.Files[fileKey];
                    Console.WriteLine($"[DEBUG CREATE] Checking {fileKey}: {(imageFile != null ? $"Found ({imageFile.Length} bytes)" : "Not found")}");
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part6Group{g}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm động (câu 147-163)
                int groupDynamicIndex = 0;
                while (true)
                {
                    var fileKey = $"Part7Images_GroupDynamic[{groupDynamicIndex}]";
                    var imageFile = Request.Form.Files[fileKey];
                    if (imageFile == null || imageFile.Length == 0) break;

                    var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var imgPath = Path.Combine(imgDir, imgName);
                    using (var stream = new FileStream(imgPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    groupImagePaths[$"Part7GroupDynamic{groupDynamicIndex}"] = "/uploads/images/" + imgName;
                    groupDynamicIndex++;
                }

                // Part 7: Nhóm 1B (câu 164-175, 3 nhóm)
                for (int g = 0; g < 3; g++)
                {
                    var fileKey = $"Part7ImageGroup1B{g}";
                    var imageFile = Request.Form.Files[fileKey];
                    Console.WriteLine($"[DEBUG CREATE] Checking {fileKey}: {(imageFile != null ? $"Found ({imageFile.Length} bytes)" : "Not found")}");
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7Group1B{g}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm 2 (câu 176-185, 2 nhóm)
                for (int g = 0; g < 2; g++)
                {
                    var fileKey = $"Part7ImageGroup{g + 1}";
                    var imageFile = Request.Form.Files[fileKey];
                    Console.WriteLine($"[DEBUG CREATE] Checking {fileKey}: {(imageFile != null ? $"Found ({imageFile.Length} bytes)" : "Not found")}");
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7Group{g + 1}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm 3 (câu 186-200, 3 nhóm)
                for (int g = 0; g < 3; g++)
                {
                    var fileKey = $"Part7ImageGroup{g + 3}";
                    var imageFile = Request.Form.Files[fileKey];
                    Console.WriteLine($"[DEBUG CREATE] Checking {fileKey}: {(imageFile != null ? $"Found ({imageFile.Length} bytes)" : "Not found")}");
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7Group{g + 3}"] = "/uploads/images/" + imgName;
                    }
                }

                // Lưu quiz nếu có
                Console.WriteLine($"[DEBUG CREATE] Total quizzes received: {quizzes?.Count ?? 0}");
                Console.WriteLine($"[DEBUG CREATE] GroupImagePaths count: {groupImagePaths.Count}");
                foreach (var kvp in groupImagePaths)
                {
                    Console.WriteLine($"[DEBUG CREATE] GroupImagePath: {kvp.Key} = {kvp.Value}");
                }

                if (quizzes != null && quizzes.Any())
                {
                    for (int idx = 0; idx < quizzes.Count; idx++)
                    {
                        var quiz = quizzes[idx];

                        // Debug: Log mọi quiz
                        Console.WriteLine($"[DEBUG CREATE] Processing quiz {idx}: Question='{quiz.Question}', OptionA='{quiz.OptionA}', OptionB='{quiz.OptionB}'");

                        // Skip quiz rỗng (chỉ skip nếu HOÀN TOÀN rỗng)
                        if (string.IsNullOrWhiteSpace(quiz.Question) &&
                            string.IsNullOrWhiteSpace(quiz.OptionA) &&
                            string.IsNullOrWhiteSpace(quiz.OptionB) &&
                            string.IsNullOrWhiteSpace(quiz.OptionC) &&
                            string.IsNullOrWhiteSpace(quiz.OptionD))
                        {
                            Console.WriteLine($"[DEBUG CREATE] Skip completely empty quiz at index {idx}");
                            continue;
                        }

                        quiz.LessonId = lesson.LessonId;

                        // Lấy ImageGroup từ form (nếu có)
                        var imageGroupKey = Request.Form[$"Quizzes[{idx}].ImageGroup"].ToString();
                        Console.WriteLine($"[DEBUG CREATE] Quiz {idx}: ImageGroupKey='{imageGroupKey}'");

                        if (!string.IsNullOrEmpty(imageGroupKey) && groupImagePaths.ContainsKey(imageGroupKey))
                        {
                            quiz.ImagePath = groupImagePaths[imageGroupKey];
                            Console.WriteLine($"[DEBUG CREATE] Set ImagePath for quiz {idx}: {quiz.ImagePath}");
                        }
                        // Nếu không có nhóm, kiểm tra ImageFile riêng lẻ (cho Part 1)
                        else if (quiz.ImageFile != null && quiz.ImageFile.Length > 0)
                        {
                            var imgName = Guid.NewGuid() + Path.GetExtension(quiz.ImageFile.FileName);
                            var imgPath = Path.Combine(imgDir, imgName);

                            using (var stream = new FileStream(imgPath, FileMode.Create))
                            {
                                await quiz.ImageFile.CopyToAsync(stream);
                            }

                            quiz.ImagePath = "/uploads/images/" + imgName;
                            Console.WriteLine($"[DEBUG CREATE] Set individual ImagePath for quiz {idx}: {quiz.ImagePath}");
                        }

                        _context.Quizzes.Add(quiz);
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
                    var oldAudio = Path.Combine(Directory.GetCurrentDirectory(), "Storage", existingLesson.AudioPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldAudio))
                    {
                        System.IO.File.Delete(oldAudio);
                        Console.WriteLine($"[DEBUG] Deleted old audio: {oldAudio}");
                    }
                }

                var audioDir = Path.Combine(Directory.GetCurrentDirectory(), "Storage/uploads/audio");
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

            // =========================================================
            // ✅ FIX: Xử lý hình ảnh nhóm cho Part 6 và Part 7 (như CreateToeic)
            // =========================================================
            var groupImagePaths = new Dictionary<string, string>();
            if (existingLesson.LessonType == "TOEIC")
            {
                var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "Storage/uploads/images");
                if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);

                // Part 6: 4 nhóm
                for (int g = 0; g < 4; g++)
                {
                    var fileKey = $"Part6ImageGroup{g}";
                    var imageFile = Request.Form.Files[fileKey];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part6Group{g}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm động (check max 20 nhóm để tránh infinite loop)
                for (int g = 0; g < 20; g++)
                {
                    var fileKey = $"Part7Images_GroupDynamic[{g}]";
                    var imageFile = Request.Form.Files[fileKey];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7GroupDynamic{g}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm 1B (3 nhóm)
                for (int g = 0; g < 3; g++)
                {
                    var fileKey = $"Part7ImageGroup1B{g}";
                    var imageFile = Request.Form.Files[fileKey];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7Group1B{g}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm 2 (2 nhóm)
                for (int g = 0; g < 2; g++)
                {
                    var fileKey = $"Part7ImageGroup{g + 1}";
                    var imageFile = Request.Form.Files[fileKey];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7Group{g + 1}"] = "/uploads/images/" + imgName;
                    }
                }

                // Part 7: Nhóm 3 (3 nhóm)
                for (int g = 0; g < 3; g++)
                {
                    var fileKey = $"Part7ImageGroup{g + 3}";
                    var imageFile = Request.Form.Files[fileKey];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imgName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var imgPath = Path.Combine(imgDir, imgName);
                        using (var stream = new FileStream(imgPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        groupImagePaths[$"Part7Group{g + 3}"] = "/uploads/images/" + imgName;
                    }
                }
            }

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
                                var oldImg = Path.Combine(Directory.GetCurrentDirectory(), "Storage", quizToDelete.ImagePath.TrimStart('/'));
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

                                // ✅ Xử lý ảnh (ưu tiên Group trước, sau đó mới tới ImageFile lẻ)
                                bool imageUpdated = false;
                                string newImagePath = null;

                                // 1. Check Image Group
                                if (!string.IsNullOrEmpty(quiz.ImageGroup) && groupImagePaths.ContainsKey(quiz.ImageGroup))
                                {
                                    newImagePath = groupImagePaths[quiz.ImageGroup];
                                    imageUpdated = true;
                                }
                                // 2. Check Individual Image File
                                else if (quiz.ImageFile != null && quiz.ImageFile.Length > 0)
                                {
                                    // Tạo thư mục nếu chưa có
                                    var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "Storage/uploads/images");
                                    if (!Directory.Exists(imgDir)) Directory.CreateDirectory(imgDir);

                                    // Lưu file mới
                                    var imgName = Guid.NewGuid() + Path.GetExtension(quiz.ImageFile.FileName);
                                    var imgPath = Path.Combine(imgDir, imgName);

                                    using (var stream = new FileStream(imgPath, FileMode.Create))
                                    {
                                        await quiz.ImageFile.CopyToAsync(stream);
                                    }
                                    newImagePath = "/uploads/images/" + imgName;
                                    imageUpdated = true;
                                }

                                // 3. Apply Update if needed
                                if (imageUpdated)
                                {
                                    // Xóa ảnh cũ nếu tồn tại (và khác ảnh mới - dù GUID luôn khác nhưng check cho chắc)
                                    // Lưu ý: Với Image Group, nhiều câu hỏi chia sẻ 1 ảnh, nên xóa ảnh cũ có thể ảnh hưởng câu khác nếu chúng chưa được update link?
                                    // Tuy nhiên, logic ở đây là: Nếu 1 Group có ảnh mới -> Tất cả câu trong group đó (khi loop qua) sẽ được gán ảnh mới.
                                    // Ảnh cũ sẽ không còn ai dùng (trừ khi có câu hỏi nào đó thuộc group này mà KHÔNG được submit trong form? Không thể, vì form submit full list).
                                    // Tuy nhiên, an toàn nhất là chỉ xóa nếu đó KHÔNG phải là ảnh đang được dùng bởi câu hỏi khác trong DB mà ta chưa đụng tới?
                                    // Nhưng đây là edit full list.
                                    // Để an toàn và đơn giản: cứ xóa ảnh cũ nếu path cũ có giá trị.
                                    // (Cẩn thận: nếu 4 câu cùng chia sẻ 1 ảnh cũ, câu 1 update -> xóa ảnh cũ. Câu 2 update -> ảnh cũ đã mất -> không sao cả).

                                    if (!string.IsNullOrEmpty(existingQuiz.ImagePath))
                                    {
                                        // Chỉ xóa nếu ảnh cũ không phải là ảnh mới (trường hợp hiếm)
                                        if (existingQuiz.ImagePath != newImagePath)
                                        {
                                            try 
                                            {
                                                var oldImg = Path.Combine(Directory.GetCurrentDirectory(), "Storage", existingQuiz.ImagePath.TrimStart('/'));
                                                // CHÚ Ý: Logic xóa file này có thể rủi ro nếu nhiều câu dùng chung 1 file ảnh cũ.
                                                // Khi loop duyệt qua câu đầu tiên của nhóm, nó xóa ảnh cũ.
                                                // Các câu sau vẫn trỏ tới ảnh cũ đó (trong DB) nhưng file đã mất? Không sao, vì chúng sẽ được update sang ảnh mới ngay lập tức.
                                                // Vấn đề duy nhất: Nếu ta xóa file, mà việc update DB thất bại sau đó? Transaction sẽ rollback DB, nhưng file đã mất.
                                                // Chấp nhận rủi ro này hoặc không xóa file cũ.
                                                // Để an toàn tuyệt đối cho bài toán chia sẻ ảnh: Tạm thời KHÔNG xóa ảnh cũ khi update group, hoặc phải check kỹ hơn.
                                                // Nhưng Part 1 dùng ảnh riêng, xóa ok.
                                                
                                                // Quyết định: Vẫn xóa, nhưng cẩn thận.
                                                // Nếu là Group Image -> các câu khác trong group cũng sẽ update sang ảnh mới trong cùng request này.
                                                
                                                if (System.IO.File.Exists(oldImg))
                                                {
                                                     // Tạm comment việc xóa file để tránh lỗi mất ảnh nếu cơ chế group phức tạp
                                                     // System.IO.File.Delete(oldImg); 
                                                }
                                            }
                                            catch { /* Ignore delete error */ }
                                        }
                                    }

                                    existingQuiz.ImagePath = newImagePath;
                                }

                                _context.Quizzes.Update(existingQuiz);
                            }
                        }
                        else
                        {
                            // Add new quiz
                            quiz.LessonId = lesson.LessonId;

                            // ✅ Set image (Group hoặc Single)
                            if (!string.IsNullOrEmpty(quiz.ImageGroup) && groupImagePaths.ContainsKey(quiz.ImageGroup))
                            {
                                quiz.ImagePath = groupImagePaths[quiz.ImageGroup];
                            }
                            else if (quiz.ImageFile != null && quiz.ImageFile.Length > 0)
                            {
                                var imgDir = Path.Combine(Directory.GetCurrentDirectory(), "Storage/uploads/images");
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
                    var audioPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", lesson.AudioPath.TrimStart('/'));
                    if (System.IO.File.Exists(audioPath))
                        System.IO.File.Delete(audioPath);
                }

                if (!string.IsNullOrEmpty(lesson.VideoUrl))
                {
                    var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", lesson.VideoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(videoPath))
                        System.IO.File.Delete(videoPath);
                }

                foreach (var quiz in lesson.Quizzes)
                {
                    if (!string.IsNullOrEmpty(quiz.ImagePath))
                    {
                        var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", quiz.ImagePath.TrimStart('/'));
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
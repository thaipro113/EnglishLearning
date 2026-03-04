# Debug Edit TOEIC - Hình ảnh không lưu vào DB

## Vấn đề
Khi edit đề TOEIC và chọn hình ảnh mới cho Part 6 hoặc Part 7, hình ảnh không được lưu vào database.

## Nguyên nhân có thể
1. **Model Binding Gap**: Giống như Create, nếu có gap trong array index, các quiz sau gap không được bind
2. **ImageGroup không được gửi đúng**: Form không gửi ImageGroup key
3. **Controller không nhận được file**: Request.Form.Files không có file
4. **Logic update không đúng**: Code update quiz nhưng không save ImagePath

## Giải pháp đã áp dụng

### 1. Thêm Hidden Inputs cho câu 147-163 trong Edit.cshtml

```razor
<div id="part7-147-163-container">
    @{
        // Tạo sẵn hidden inputs cho tất cả câu 147-163 để tránh gap
        for (int i = 146; i < 163; i++)
        {
            var quiz = Model.Quizzes.ElementAtOrDefault(i);
            // Nếu quiz không tồn tại hoặc rỗng, tạo hidden inputs
            if (quiz == null || string.IsNullOrWhiteSpace(quiz.Question))
            {
                <input type="hidden" name="Quizzes[@i].QuizId" value="0" />
                <input type="hidden" name="Quizzes[@i].LessonId" value="@Model.LessonId" />
                <input type="hidden" name="Quizzes[@i].Question" value="" />
                <input type="hidden" name="Quizzes[@i].OptionA" value="" />
                <input type="hidden" name="Quizzes[@i].OptionB" value="" />
                <input type="hidden" name="Quizzes[@i].OptionC" value="" />
                <input type="hidden" name="Quizzes[@i].OptionD" value="" />
                <input type="hidden" name="Quizzes[@i].CorrectAnswer" value="" />
                <input type="hidden" name="Quizzes[@i].ImageGroup" value="" />
            }
        }
        // ... hiển thị các nhóm đã có ...
    }
</div>
```

### 2. Thêm Debug Logging trong LessonController.cs

```csharp
Console.WriteLine($"[DEBUG EDIT] Total quizzes received: {quizzes?.Count ?? 0}");
Console.WriteLine($"[DEBUG EDIT] GroupImagePaths count: {groupImagePaths.Count}");
foreach (var kvp in groupImagePaths)
{
    Console.WriteLine($"[DEBUG EDIT] GroupImagePath: {kvp.Key} = {kvp.Value}");
}

// Trong loop xử lý quiz
var imageGroupKey = Request.Form[$"Quizzes[{idx}].ImageGroup"].ToString();
Console.WriteLine($"[DEBUG EDIT] Quiz {idx} (ID: {quiz.QuizId}): ImageGroupKey = '{imageGroupKey}'");

if (!string.IsNullOrEmpty(imageGroupKey) && groupImagePaths.ContainsKey(imageGroupKey))
{
    var newImagePath = groupImagePaths[imageGroupKey];
    Console.WriteLine($"[DEBUG EDIT] Updating quiz {idx} ImagePath: {existingQuiz.ImagePath} -> {newImagePath}");
    existingQuiz.ImagePath = newImagePath;
}
```

### 3. Skip Empty Quizzes

```csharp
// Skip quiz rỗng (từ hidden inputs)
if (string.IsNullOrWhiteSpace(quiz.Question) || 
    (string.IsNullOrWhiteSpace(quiz.OptionA) && 
     string.IsNullOrWhiteSpace(quiz.OptionB) && 
     string.IsNullOrWhiteSpace(quiz.OptionC)))
{
    Console.WriteLine($"[DEBUG EDIT] Skip empty quiz at index {idx}");
    continue;
}
```

## Cách Debug

### Bước 1: Kiểm tra Console Log

Khi submit form Edit, kiểm tra console output (Visual Studio Output window hoặc terminal):

```
[DEBUG EDIT] Total quizzes received: 200
[DEBUG EDIT] GroupImagePaths count: 4
[DEBUG EDIT] GroupImagePath: Part6Group0 = /uploads/images/abc123.jpg
[DEBUG EDIT] GroupImagePath: Part6Group1 = /uploads/images/def456.jpg
...
[DEBUG EDIT] Quiz 130 (ID: 1001): ImageGroupKey = 'Part6Group0'
[DEBUG EDIT] Updating quiz 130 ImagePath: /uploads/images/old.jpg -> /uploads/images/abc123.jpg
```

### Bước 2: Kiểm tra Request.Form.Files

Thêm debug code vào controller:

```csharp
Console.WriteLine($"[DEBUG] Total files in request: {Request.Form.Files.Count}");
foreach (var file in Request.Form.Files)
{
    Console.WriteLine($"[DEBUG] File: {file.Name}, Length: {file.Length}");
}
```

Kết quả mong đợi:
```
[DEBUG] Total files in request: 8
[DEBUG] File: Part6ImageGroup0, Length: 123456
[DEBUG] File: Part6ImageGroup1, Length: 234567
[DEBUG] File: Part7ImageGroup1B0, Length: 345678
...
```

### Bước 3: Kiểm tra Database

Sau khi submit, kiểm tra database:

```sql
-- Kiểm tra ImagePath của các quiz trong nhóm Part 6
SELECT QuizId, Question, ImagePath 
FROM Quizzes 
WHERE LessonId = 'TOEIC001' 
  AND QuizId BETWEEN 1001 AND 1016
ORDER BY QuizId;

-- Kiểm tra ImagePath của các quiz trong nhóm Part 7
SELECT QuizId, Question, ImagePath 
FROM Quizzes 
WHERE LessonId = 'TOEIC001' 
  AND QuizId BETWEEN 1017 AND 1070
ORDER BY QuizId;
```

### Bước 4: Kiểm tra File Upload

Kiểm tra thư mục `wwwroot/uploads/images/` xem có file mới được tạo không:

```powershell
# Liệt kê các file mới nhất
Get-ChildItem "wwwroot/uploads/images/" | Sort-Object LastWriteTime -Descending | Select-Object -First 10
```

## Các trường hợp lỗi thường gặp

### Lỗi 1: GroupImagePaths count = 0

**Nguyên nhân**: Form không gửi file hoặc file name không đúng

**Giải pháp**: 
- Kiểm tra `enctype="multipart/form-data"` trong form tag
- Kiểm tra name của input file: `name="Part6ImageGroup0"`
- Kiểm tra file có được chọn không

### Lỗi 2: Quiz ImageGroupKey = ''

**Nguyên nhân**: Hidden input ImageGroup không được gửi

**Giải pháp**:
- Kiểm tra có hidden input: `<input type="hidden" name="Quizzes[130].ImageGroup" value="Part6Group0" />`
- Kiểm tra value không rỗng

### Lỗi 3: Total quizzes received < 200

**Nguyên nhân**: Model binding gap

**Giải pháp**:
- Đã fix bằng cách thêm hidden inputs cho câu 147-163
- Kiểm tra không có gap nào khác

### Lỗi 4: ImagePath không update trong DB

**Nguyên nhân**: 
- Không gọi `_context.Quizzes.Update(existingQuiz)`
- Không gọi `await _context.SaveChangesAsync()`

**Giải pháp**:
- Đảm bảo có `_context.Quizzes.Update(existingQuiz)` sau khi set ImagePath
- Đảm bảo có `await _context.SaveChangesAsync()` ở cuối

## Test Case

### Test 1: Edit Part 6 - Thay đổi hình nhóm 1

1. Truy cập: `/Admin/Lesson/Edit/TOEIC001`
2. Scroll đến Part 6, Nhóm 1 (Câu 131-134)
3. Click "Chọn tệp" và chọn hình mới
4. Xem preview hiển thị
5. Click "Cập nhật"
6. Kiểm tra database: ImagePath của câu 131-134 phải giống nhau và là path mới

### Test 2: Edit Part 7 - Thay đổi hình nhóm 1B

1. Truy cập: `/Admin/Lesson/Edit/TOEIC001`
2. Scroll đến Part 7, Nhóm 1B, Hình 1 (Câu 164-167)
3. Click "Chọn tệp" và chọn hình mới
4. Xem preview hiển thị
5. Click "Cập nhật"
6. Kiểm tra database: ImagePath của câu 164-167 phải giống nhau và là path mới

### Test 3: Edit nhiều nhóm cùng lúc

1. Truy cập: `/Admin/Lesson/Edit/TOEIC001`
2. Thay đổi hình cho:
   - Part 6 Nhóm 1
   - Part 6 Nhóm 2
   - Part 7 Nhóm 1B Hình 1
   - Part 7 Nhóm 2 Hình 1
3. Click "Cập nhật"
4. Kiểm tra database: Tất cả các nhóm phải có ImagePath mới

## Kết quả mong đợi

✅ Khi chọn hình mới, preview hiển thị ngay
✅ Khi submit, console log hiển thị đúng số quiz và GroupImagePaths
✅ Database được update với ImagePath mới
✅ File hình được lưu vào `wwwroot/uploads/images/`
✅ Các quiz trong cùng nhóm có ImagePath giống nhau
✅ File hình cũ được xóa (nếu không có quiz nào khác dùng)

## Nếu vẫn lỗi

Nếu sau khi áp dụng các fix trên mà vẫn lỗi, hãy:

1. **Chụp màn hình console log** và gửi cho tôi
2. **Kiểm tra database** trước và sau khi edit
3. **Kiểm tra file system** xem có file mới được tạo không
4. **Kiểm tra browser Network tab** xem request có gửi file không

Tôi sẽ phân tích và đưa ra giải pháp cụ thể hơn.

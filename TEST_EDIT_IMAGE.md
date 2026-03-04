# Test Edit TOEIC Image Upload

## Bước test chi tiết

### 1. Mở Console Output
- Trong Visual Studio: View → Output → Show output from: Debug
- Hoặc chạy từ terminal để xem console log

### 2. Edit một đề TOEIC
1. Truy cập: `http://localhost:44335/Admin/Lesson/Edit/TOEIC001` (thay TOEIC001 bằng ID thực tế)
2. Scroll xuống Part 6, Nhóm 1 (Câu 131-134)
3. Click "Chọn tệp" và chọn một hình ảnh
4. Xem preview có hiển thị không
5. Click "Cập nhật"

### 3. Kiểm tra Console Log

Bạn sẽ thấy output như sau:

```
[DEBUG EDIT] Total quizzes received: 200
[DEBUG EDIT] GroupImagePaths count: 1
[DEBUG EDIT] GroupImagePath: Part6Group0 = /uploads/images/abc-123-def.jpg
[DEBUG EDIT] Quiz 130 (ID: 1001): ImageGroupKey = 'Part6Group0'
[DEBUG EDIT] Updating quiz 130 ImagePath: /uploads/images/old.jpg -> /uploads/images/abc-123-def.jpg
[DEBUG EDIT] Quiz 131 (ID: 1002): ImageGroupKey = 'Part6Group0'
[DEBUG EDIT] Updating quiz 131 ImagePath: /uploads/images/old.jpg -> /uploads/images/abc-123-def.jpg
[DEBUG EDIT] Quiz 132 (ID: 1003): ImageGroupKey = 'Part6Group0'
[DEBUG EDIT] Updating quiz 132 ImagePath: /uploads/images/old.jpg -> /uploads/images/abc-123-def.jpg
[DEBUG EDIT] Quiz 133 (ID: 1004): ImageGroupKey = 'Part6Group0'
[DEBUG EDIT] Updating quiz 133 ImagePath: /uploads/images/old.jpg -> /uploads/images/abc-123-def.jpg
```

### 4. Các trường hợp lỗi

#### Lỗi A: GroupImagePaths count = 0
```
[DEBUG EDIT] Total quizzes received: 200
[DEBUG EDIT] GroupImagePaths count: 0
```

**Nguyên nhân**: File không được upload

**Kiểm tra**:
1. Form có `enctype="multipart/form-data"` không?
2. Input file có name đúng không? Phải là `Part6ImageGroup0`, `Part6ImageGroup1`, etc.
3. File có được chọn không?

**Fix**: Kiểm tra Edit.cshtml

#### Lỗi B: ImageGroupKey = ''
```
[DEBUG EDIT] Quiz 130 (ID: 1001): ImageGroupKey = ''
```

**Nguyên nhân**: Hidden input ImageGroup không được gửi

**Kiểm tra**: Xem source HTML (F12 → Elements), tìm:
```html
<input type="hidden" name="Quizzes[130].ImageGroup" value="Part6Group0" />
```

**Fix**: Kiểm tra Edit.cshtml có hidden input này không

#### Lỗi C: Total quizzes received < 200
```
[DEBUG EDIT] Total quizzes received: 146
```

**Nguyên nhân**: Model binding gap - các quiz sau index 146 không được bind

**Fix**: Đã fix bằng cách thêm hidden inputs cho câu 147-163

#### Lỗi D: Quiz không có trong log
```
[DEBUG EDIT] Quiz 130 (ID: 1001): ImageGroupKey = 'Part6Group0'
[DEBUG EDIT] Skip empty quiz at index 131
[DEBUG EDIT] Skip empty quiz at index 132
```

**Nguyên nhân**: Quiz bị skip vì rỗng

**Fix**: Kiểm tra form có đủ dữ liệu không

### 5. Kiểm tra Database

Sau khi submit, chạy query:

```sql
-- Kiểm tra ImagePath của Part 6 Nhóm 1 (câu 131-134)
SELECT QuizId, Question, ImagePath 
FROM Quizzes 
WHERE LessonId = 'TOEIC001' 
ORDER BY QuizId
LIMIT 4 OFFSET 130;
```

Kết quả mong đợi: 4 quiz có ImagePath giống nhau và là path mới.

### 6. Kiểm tra File System

```powershell
# Liệt kê file mới nhất trong thư mục uploads
Get-ChildItem "EnglishLearning/wwwroot/uploads/images/" | 
    Sort-Object LastWriteTime -Descending | 
    Select-Object Name, Length, LastWriteTime -First 5
```

Phải có file mới được tạo với timestamp gần đây.

## Nếu vẫn lỗi - Thêm debug code

Nếu sau khi test mà vẫn lỗi, thêm debug code này vào `LessonController.cs`, ngay sau dòng `var groupImagePaths = new Dictionary<string, string>();`:

```csharp
// Debug: Log tất cả files trong request
Console.WriteLine($"[DEBUG] Total files in request: {Request.Form.Files.Count}");
foreach (var file in Request.Form.Files)
{
    Console.WriteLine($"[DEBUG] File name: {file.Name}, FileName: {file.FileName}, Length: {file.Length}");
}

// Debug: Log tất cả form keys có chứa ImageGroup
Console.WriteLine($"[DEBUG] Form keys with ImageGroup:");
foreach (var key in Request.Form.Keys.Where(k => k.Contains("ImageGroup")))
{
    Console.WriteLine($"[DEBUG]   {key} = {Request.Form[key]}");
}
```

Sau đó test lại và gửi cho tôi console output.

## Quick Fix - Nếu cần fix ngay

Nếu bạn cần fix ngay và không muốn debug, có thể thử cách này:

### Cách 1: Đảm bảo form có enctype

Kiểm tra Edit.cshtml, dòng `<form>` phải có:

```razor
<form asp-action="Edit" method="post" enctype="multipart/form-data" class="custom-form">
```

### Cách 2: Đảm bảo input file có name đúng

Kiểm tra Edit.cshtml, input file phải có name chính xác:

```razor
<!-- Part 6 -->
<input type="file" name="Part6ImageGroup0" id="Part6ImageGroup0" accept="image/*" class="form-control" />
<input type="file" name="Part6ImageGroup1" id="Part6ImageGroup1" accept="image/*" class="form-control" />
<input type="file" name="Part6ImageGroup2" id="Part6ImageGroup2" accept="image/*" class="form-control" />
<input type="file" name="Part6ImageGroup3" id="Part6ImageGroup3" accept="image/*" class="form-control" />

<!-- Part 7 Nhóm 1B -->
<input type="file" name="Part7ImageGroup1B0" id="Part7ImageGroup1B0" accept="image/*" class="form-control" />
<input type="file" name="Part7ImageGroup1B1" id="Part7ImageGroup1B1" accept="image/*" class="form-control" />
<input type="file" name="Part7ImageGroup1B2" id="Part7ImageGroup1B2" accept="image/*" class="form-control" />
```

### Cách 3: Đảm bảo hidden input ImageGroup có value

Kiểm tra Edit.cshtml, mỗi quiz phải có hidden input:

```razor
<input type="hidden" name="Quizzes[130].ImageGroup" value="Part6Group0" />
<input type="hidden" name="Quizzes[131].ImageGroup" value="Part6Group0" />
<input type="hidden" name="Quizzes[132].ImageGroup" value="Part6Group0" />
<input type="hidden" name="Quizzes[133].ImageGroup" value="Part6Group0" />
```

## Gửi cho tôi

Nếu vẫn lỗi, hãy gửi cho tôi:
1. **Console output** (copy toàn bộ từ khi click "Cập nhật")
2. **Screenshot** của form Edit (phần Part 6 hoặc Part 7)
3. **Database query result** (SELECT * FROM Quizzes WHERE LessonId = 'TOEIC001' LIMIT 20 OFFSET 130)

Tôi sẽ phân tích và fix chính xác hơn.

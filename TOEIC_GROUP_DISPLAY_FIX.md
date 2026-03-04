# Sửa hiển thị nhóm câu hỏi TOEIC

## Vấn đề
Khi làm bài TOEIC, các câu hỏi Part 6 và Part 7 có nhiều câu dùng chung 1 hình, nhưng chỉ hiển thị 1 câu mỗi lần. Người dùng phải click nhiều lần để xem các câu trong cùng nhóm.

## Yêu cầu
1. **Hiển thị nhóm câu hỏi**: Khi click vào 1 câu trong nhóm, hiển thị tất cả câu trong nhóm đó cùng với hình ảnh
2. **Tô xanh dương các câu trong nhóm**: Trong sơ đồ câu hỏi bên trái, tô xanh dương tất cả các số câu đang hiển thị

## Giải pháp đã áp dụng

### 1. Cập nhật function `showQuestion()`

**Trước:**
- Chỉ hiển thị 1 câu hỏi mỗi lần
- Chỉ tô xanh 1 số câu

**Sau:**
```javascript
function showQuestion(index) {
    // Lấy imageGroup của câu hỏi
    const currentQ = questions[index];
    const imageGroup = currentQ.getAttribute('data-image-group');
    
    // Nếu có imageGroup, tìm tất cả câu trong nhóm
    if (imageGroup && imageGroup.trim() !== '') {
        questionsToShow = [];
        questions.forEach((q, i) => {
            if (q.getAttribute('data-image-group') === imageGroup) {
                questionsToShow.push(i);
            }
        });
        
        // Hiển thị tất cả câu trong nhóm
        questionsToShow.forEach(i => {
            questions[i].style.display = 'block';
        });
        
        // Tô xanh dương tất cả số câu trong nhóm
        qNumbers.forEach((n, i) => {
            if (questionsToShow.includes(i)) {
                n.classList.add('current');
            }
        });
    }
}
```

### 2. Cập nhật function `nextQuestion()` và `prevQuestion()`

**Trước:**
- Nhảy từng câu một

**Sau:**
- Nhảy theo nhóm câu hỏi
- Khi click "Câu tiếp theo", nhảy đến nhóm tiếp theo (không phải câu tiếp theo)

```javascript
function nextQuestion() {
    // Tìm câu cuối cùng trong nhóm hiện tại
    let lastInGroup = currentQuestion;
    for (let i = currentQuestion + 1; i < totalQuestions; i++) {
        if (questions[i].getAttribute('data-image-group') === currentImageGroup) {
            lastInGroup = i;
        } else {
            break;
        }
    }
    // Nhảy đến câu đầu tiên của nhóm tiếp theo
    if (lastInGroup < totalQuestions - 1) {
        showQuestion(lastInGroup + 1);
    }
}
```

### 3. Cập nhật CSS

**Tô xanh dương đậm hơn:**
```css
.q-number.current {
    border-color: #2196F3;
    background: #2196F3;  /* Xanh dương đậm */
    color: white;
    font-weight: bold;
}

.q-number.current.answered {
    background: #1976D2;  /* Xanh dương đậm hơn nếu đã trả lời */
    border-color: #1565C0;
    color: white;
}
```

**Highlight các câu hỏi trong nhóm:**
```css
.question-item[data-image-group]:not([data-image-group=""]) {
    border-left: 4px solid #2196F3;
    background: #f8f9fa;
}
```

## Cách hoạt động

### Part 6: Text Completion (4 câu/hình)

**Trước:**
```
Click câu 131 → Hiển thị câu 131
Click câu 132 → Hiển thị câu 132
Click câu 133 → Hiển thị câu 133
Click câu 134 → Hiển thị câu 134
```

**Sau:**
```
Click câu 131 → Hiển thị câu 131, 132, 133, 134 cùng lúc
                Tô xanh dương số 131, 132, 133, 134
Click "Câu tiếp theo" → Nhảy đến câu 135 (nhóm tiếp theo)
```

### Part 7: Reading Comprehension (2-5 câu/hình)

**Nhóm 1B (4 câu/hình):**
```
Click câu 164 → Hiển thị câu 164, 165, 166, 167
                Tô xanh dương số 164, 165, 166, 167
```

**Nhóm 2 (5 câu/hình):**
```
Click câu 176 → Hiển thị câu 176, 177, 178, 179, 180
                Tô xanh dương số 176, 177, 178, 179, 180
```

## Lợi ích

1. ✅ **Tiết kiệm thời gian**: Không cần click nhiều lần để xem các câu trong cùng nhóm
2. ✅ **Dễ so sánh**: Có thể xem tất cả câu hỏi liên quan đến 1 hình cùng lúc
3. ✅ **Trực quan hơn**: Tô xanh dương giúp biết đang ở nhóm nào
4. ✅ **Giống thi thật**: Trong đề thi TOEIC thật, các câu cùng nhóm được in trên cùng 1 trang

## Test

### Test 1: Part 6 - Nhóm 1 (Câu 131-134)
1. Truy cập: `/Test/DoTest/{lessonId}`
2. Scroll đến Part 6
3. Click số 131 trong sơ đồ câu hỏi
4. **Kết quả mong đợi:**
   - Hiển thị câu 131, 132, 133, 134 cùng lúc
   - Hình ảnh hiển thị ở trên
   - Số 131, 132, 133, 134 được tô xanh dương

### Test 2: Part 7 - Nhóm 1B (Câu 164-167)
1. Click số 164
2. **Kết quả mong đợi:**
   - Hiển thị câu 164, 165, 166, 167
   - Số 164, 165, 166, 167 được tô xanh dương

### Test 3: Navigation
1. Đang ở câu 131 (nhóm 131-134)
2. Click "Câu tiếp theo"
3. **Kết quả mong đợi:**
   - Nhảy đến câu 135 (nhóm tiếp theo)
   - Không nhảy đến câu 132

### Test 4: Click trực tiếp vào số câu
1. Click số 132 (trong nhóm 131-134)
2. **Kết quả mong đợi:**
   - Hiển thị cả nhóm 131-134
   - Tô xanh dương cả 4 số

## Files đã sửa

- ✅ `EnglishLearning/Views/Test/DoTestToeic.cshtml`
  - Function `showQuestion()`: Hiển thị nhóm câu hỏi
  - Function `nextQuestion()`: Nhảy theo nhóm
  - Function `prevQuestion()`: Nhảy theo nhóm
  - CSS: Tô xanh dương và highlight

## Lưu ý

- Các câu không có `data-image-group` (Part 1-5) vẫn hiển thị từng câu một như cũ
- Chỉ các câu có `data-image-group` (Part 6-7) mới hiển thị theo nhóm
- Khi trả lời câu trong nhóm, số câu vẫn được đánh dấu xanh lá (answered) như bình thường

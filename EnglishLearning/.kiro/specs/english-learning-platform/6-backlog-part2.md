# 6. PROJECT BACKLOG (CHI TIẾT) - PHẦN 2

## 6. Epic 5: Bài Kiểm Tra

### Epic 5.1: Làm Bài Kiểm Tra

#### Story 5.1.1: Làm Bài Kiểm Tra
**ID**: US-021  
**Ưu Tiên**: Cao  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng làm bài kiểm tra để đánh giá kiến thức

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị danh sách bài kiểm tra
- [ ] Hiển thị câu hỏi và các lựa chọn
- [ ] Chọn đáp án
- [ ] Chuyển sang câu hỏi tiếp theo
- [ ] Nộp bài
- [ ] Tính điểm
- [ ] Hiển thị kết quả (điểm, phần trăm đúng)
- [ ] Lưu lịch sử bài kiểm tra

**Tasks**:
- [ ] T-085: Tạo QuizController.Index()
- [ ] T-086: Tạo QuizController.TakeQuiz()
- [ ] T-087: Tạo view danh sách bài kiểm tra
- [ ] T-088: Tạo view làm bài kiểm tra
- [ ] T-089: Tạo logic tính điểm
- [ ] T-090: Tạo view kết quả
- [ ] T-091: Viết unit tests

---

#### Story 5.1.2: TOEIC Test
**ID**: US-022  
**Ưu Tiên**: Cao  
**Effort**: XL  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Bài kiểm tra TOEIC chuyên biệt

**Tiêu Chí Chấp Nhận**:
- [ ] Bài kiểm tra TOEIC đầy đủ (Listening, Reading)
- [ ] Tính điểm theo thang TOEIC (10-990)
- [ ] Phân tích kết quả chi tiết
- [ ] So sánh với kết quả trước đó
- [ ] Lưu lịch sử bài thi

**Tasks**:
- [ ] T-092: Tạo TOEIC test model
- [ ] T-093: Tạo QuizController.TOEICTest()
- [ ] T-094: Tạo logic tính điểm TOEIC
- [ ] T-095: Tạo view TOEIC test
- [ ] T-096: Tạo view phân tích kết quả
- [ ] T-097: Viết unit tests

---

### Epic 5.2: Quản Lý Bài Kiểm Tra (Admin)

#### Story 5.2.1: Tạo Bài Kiểm Tra
**ID**: US-023  
**Ưu Tiên**: Cao  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin tạo bài kiểm tra mới

**Tiêu Chí Chấp Nhận**:
- [ ] Nhập tên bài kiểm tra
- [ ] Nhập mô tả
- [ ] Chọn khóa học
- [ ] Đặt thời gian giới hạn
- [ ] Thêm câu hỏi
- [ ] Bài kiểm tra được tạo thành công

**Tasks**:
- [ ] T-098: Tạo Admin/QuizController.Create()
- [ ] T-099: Tạo view tạo bài kiểm tra
- [ ] T-100: Tạo Admin/QuizController.AddQuestion()
- [ ] T-101: Tạo view thêm câu hỏi
- [ ] T-102: Viết unit tests

---

#### Story 5.2.2: Chỉnh Sửa Bài Kiểm Tra
**ID**: US-024  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin chỉnh sửa bài kiểm tra

**Tiêu Chí Chấp Nhận**:
- [ ] Chỉnh sửa tên, mô tả, thời gian
- [ ] Chỉnh sửa câu hỏi
- [ ] Xóa câu hỏi
- [ ] Thay đổi được lưu ngay lập tức

**Tasks**:
- [ ] T-103: Tạo Admin/QuizController.Edit()
- [ ] T-104: Tạo view chỉnh sửa bài kiểm tra
- [ ] T-105: Viết unit tests

---

#### Story 5.2.3: Xem Kết Quả Bài Kiểm Tra
**ID**: US-025  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin xem kết quả bài kiểm tra của người dùng

**Tiêu Chí Chấp Nhận**:
- [ ] Xem danh sách người dùng đã làm bài
- [ ] Xem chi tiết kết quả từng người dùng
- [ ] Xem câu trả lời của người dùng
- [ ] Xuất báo cáo

**Tasks**:
- [ ] T-106: Tạo Admin/QuizController.Results()
- [ ] T-107: Tạo view danh sách kết quả
- [ ] T-108: Tạo view chi tiết kết quả
- [ ] T-109: Viết unit tests

---

## 7. Epic 6: Từ Điển

### Epic 6.1: Tra Cứu Từ Điển

#### Story 6.1.1: Tra Cứu Từ
**ID**: US-026  
**Ưu Tiên**: Trung Bình  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng tra cứu từ vựng

**Tiêu Chí Chấp Nhận**:
- [ ] Tìm kiếm từ
- [ ] Hiển thị định nghĩa
- [ ] Hiển thị ví dụ
- [ ] Hiển thị phát âm (IPA)
- [ ] Hiển thị từ đồng nghĩa, trái nghĩa
- [ ] Lưu từ yêu thích
- [ ] Tìm kiếm nhanh (< 500ms)

**Tasks**:
- [ ] T-110: Tạo DictionaryController.Search()
- [ ] T-111: Tạo view tra cứu từ
- [ ] T-112: Tạo logic tìm kiếm
- [ ] T-113: Thêm lưu từ yêu thích
- [ ] T-114: Viết unit tests

---

### Epic 6.2: Quản Lý Từ Điển (Admin)

#### Story 6.2.1: Thêm Từ Vào Từ Điển
**ID**: US-027  
**Ưu Tiên**: Trung Bình  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin thêm từ vào từ điển

**Tiêu Chí Chấp Nhận**:
- [ ] Nhập từ
- [ ] Nhập định nghĩa
- [ ] Nhập ví dụ
- [ ] Nhập phát âm
- [ ] Từ được thêm thành công

**Tasks**:
- [ ] T-115: Tạo Admin/DictionaryController.AddWord()
- [ ] T-116: Tạo view thêm từ
- [ ] T-117: Viết unit tests

---

#### Story 6.2.2: Import Từ từ File
**ID**: US-028  
**Ưu Tiên**: Trung Bình  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin import từ từ file CSV/Excel

**Tiêu Chí Chấp Nhận**:
- [ ] Upload file CSV/Excel
- [ ] Parse file
- [ ] Tạo từ từ dữ liệu
- [ ] Thông báo thành công

**Tasks**:
- [ ] T-118: Tạo Admin/DictionaryController.ImportWords()
- [ ] T-119: Tạo CSV/Excel parser
- [ ] T-120: Tạo view import
- [ ] T-121: Viết unit tests

---

## 8. Epic 7: Kiểm Tra Ngữ Pháp

### Epic 7.1: Kiểm Tra Ngữ Pháp

#### Story 7.1.1: Kiểm Tra Ngữ Pháp
**ID**: US-029  
**Ưu Tiên**: Trung Bình  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng kiểm tra ngữ pháp của văn bản

**Tiêu Chí Chấp Nhận**:
- [ ] Nhập hoặc dán văn bản
- [ ] Kiểm tra lỗi ngữ pháp
- [ ] Xem gợi ý sửa lỗi
- [ ] Xem giải thích lỗi
- [ ] Phát hiện lỗi chính xác
- [ ] Gợi ý sửa lỗi hợp lý

**Tasks**:
- [ ] T-122: Tạo GrammarController.Check()
- [ ] T-123: Tạo view kiểm tra ngữ pháp
- [ ] T-124: Tích hợp API kiểm tra ngữ pháp
- [ ] T-125: Tạo logic phân tích lỗi
- [ ] T-126: Viết unit tests

---

## 9. Epic 8: Theo Dõi Tiến Độ

### Epic 8.1: Xem Tiến Độ Cá Nhân

#### Story 8.1.1: Xem Tiến Độ Khóa Học
**ID**: US-030  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem tiến độ khóa học

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị danh sách khóa học
- [ ] Hiển thị tiến độ (X/Y bài học)
- [ ] Hiển thị biểu đồ tiến độ
- [ ] Cập nhật real-time

**Tasks**:
- [ ] T-127: Tạo ProgressController.CourseProgress()
- [ ] T-128: Tạo view tiến độ khóa học
- [ ] T-129: Tạo biểu đồ tiến độ
- [ ] T-130: Viết unit tests

---

#### Story 8.1.2: Xem Thống Kê Bài Kiểm Tra
**ID**: US-031  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem thống kê bài kiểm tra

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị danh sách bài kiểm tra
- [ ] Hiển thị điểm số
- [ ] Hiển thị biểu đồ điểm
- [ ] So sánh với kết quả trước đó

**Tasks**:
- [ ] T-131: Tạo ProgressController.QuizStats()
- [ ] T-132: Tạo view thống kê bài kiểm tra
- [ ] T-133: Tạo biểu đồ điểm
- [ ] T-134: Viết unit tests

---

#### Story 8.1.3: Xem Lịch Sử Dịch Thuật
**ID**: US-032  
**Ưu Tiên**: Trung Bình  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem lịch sử dịch thuật

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị danh sách dịch thuật
- [ ] Hiển thị ngày dịch
- [ ] Có thể xóa lịch sử

**Tasks**:
- [ ] T-135: Tạo ProgressController.TranslationHistory()
- [ ] T-136: Tạo view lịch sử dịch thuật
- [ ] T-137: Viết unit tests

---

### Epic 8.2: Quản Lý Tiến Độ (Admin)

#### Story 8.2.1: Xem Tiến Độ Người Dùng
**ID**: US-033  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin xem tiến độ của từng người dùng

**Tiêu Chí Chấp Nhận**:
- [ ] Xem danh sách người dùng
- [ ] Xem tiến độ từng người dùng
- [ ] Lọc theo khóa học
- [ ] Lọc theo thời gian

**Tasks**:
- [ ] T-138: Tạo Admin/ProgressController.UserProgress()
- [ ] T-139: Tạo view tiến độ người dùng
- [ ] T-140: Viết unit tests

---

#### Story 8.2.2: Xem Báo Cáo Toàn Hệ Thống
**ID**: US-034  
**Ưu Tiên**: Cao  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin xem báo cáo thống kê toàn hệ thống

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị thống kê tổng quát
- [ ] Hiển thị biểu đồ thống kê
- [ ] Lọc theo thời gian
- [ ] Xuất báo cáo (PDF/Excel)

**Tasks**:
- [ ] T-141: Tạo Admin/ReportController.Dashboard()
- [ ] T-142: Tạo view dashboard báo cáo
- [ ] T-143: Tạo logic xuất PDF/Excel
- [ ] T-144: Viết unit tests

---

## 10. Epic 9: Cơ Sở Hạ Tầng & DevOps

### Epic 9.1: Cơ Sở Dữ Liệu

#### Story 9.1.1: Thiết Lập Cơ Sở Dữ Liệu
**ID**: US-035  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Thiết lập cơ sở dữ liệu SQL Server

**Tiêu Chí Chấp Nhận**:
- [ ] Tạo database
- [ ] Tạo các bảng
- [ ] Tạo các index
- [ ] Tạo các foreign key

**Tasks**:
- [ ] T-145: Tạo migration ban đầu
- [ ] T-146: Tạo seed data
- [ ] T-147: Viết script backup

---

#### Story 9.1.2: Tối Ưu Hóa Cơ Sở Dữ Liệu
**ID**: US-036  
**Ưu Tiên**: Trung Bình  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Tối ưu hóa hiệu năng cơ sở dữ liệu

**Tiêu Chí Chấp Nhận**:
- [ ] Tạo index trên các cột thường xuyên tìm kiếm
- [ ] Tối ưu hóa query
- [ ] Cấu hình connection pooling

**Tasks**:
- [ ] T-148: Tạo index
- [ ] T-149: Tối ưu hóa query
- [ ] T-150: Cấu hình connection pooling

---

### Epic 9.2: Docker & Deployment

#### Story 9.2.1: Tạo Docker Image
**ID**: US-037  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Tạo Docker image cho ứng dụng

**Tiêu Chí Chấp Nhận**:
- [ ] Dockerfile được tạo
- [ ] Image được build thành công
- [ ] Container chạy thành công

**Tasks**:
- [ ] T-151: Tạo Dockerfile
- [ ] T-152: Tạo docker-compose.yml
- [ ] T-153: Test Docker image

---

#### Story 9.2.2: Deployment
**ID**: US-038  
**Ưu Tiên**: Cao  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Deploy ứng dụng lên production

**Tiêu Chí Chấp Nhận**:
- [ ] Ứng dụng chạy trên production
- [ ] HTTPS được cấu hình
- [ ] Backup được cấu hình
- [ ] Monitoring được cấu hình

**Tasks**:
- [ ] T-154: Cấu hình production server
- [ ] T-155: Cấu hình HTTPS
- [ ] T-156: Cấu hình backup
- [ ] T-157: Cấu hình monitoring

---

## 11. Epic 10: Testing & QA

### Epic 10.1: Unit Testing

#### Story 10.1.1: Viết Unit Tests
**ID**: US-039  
**Ưu Tiên**: Cao  
**Effort**: XL  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Viết unit tests cho tất cả các service

**Tiêu Chí Chấp Nhận**:
- [ ] Code coverage >= 80%
- [ ] Tất cả tests pass
- [ ] Tests chạy nhanh (< 5 phút)

**Tasks**:
- [ ] T-158: Viết tests cho UserService
- [ ] T-159: Viết tests cho CourseService
- [ ] T-160: Viết tests cho LessonService
- [ ] T-161: Viết tests cho FlashcardService
- [ ] T-162: Viết tests cho QuizService
- [ ] T-163: Viết tests cho ProgressService

---

### Epic 10.2: Integration Testing

#### Story 10.2.1: Viết Integration Tests
**ID**: US-040  
**Ưu Tiên**: Trung Bình  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Viết integration tests cho các API

**Tiêu Chí Chấp Nhận**:
- [ ] Tất cả API endpoints được test
- [ ] Tất cả tests pass
- [ ] Tests chạy nhanh (< 10 phút)

**Tasks**:
- [ ] T-164: Viết tests cho Account API
- [ ] T-165: Viết tests cho Course API
- [ ] T-166: Viết tests cho Lesson API
- [ ] T-167: Viết tests cho Quiz API

---

## 12. Tóm Tắt Backlog

### Thống Kê Công Việc

| Epic | Số User Story | Số Tasks | Effort Tổng |
|------|---------------|----------|------------|
| Quản Lý Người Dùng | 6 | 30 | 15M |
| Quản Lý Khóa Học | 6 | 18 | 12M |
| Quản Lý Bài Học | 3 | 9 | 8M |
| Flashcard | 3 | 9 | 8M |
| Bài Kiểm Tra | 5 | 15 | 20M |
| Từ Điển | 2 | 6 | 6M |
| Kiểm Tra Ngữ Pháp | 1 | 5 | 8M |
| Theo Dõi Tiến Độ | 4 | 12 | 10M |
| Cơ Sở Hạ Tầng | 2 | 7 | 8M |
| Testing & QA | 2 | 10 | 15M |
| **TỔNG** | **34** | **121** | **110M** |

### Lịch Trình Dự Kiến

| Sprint | Thời Gian | Epics | Effort |
|--------|----------|-------|--------|
| Sprint 1 | Tuần 1-2 | Quản Lý Người Dùng, Cơ Sở Hạ Tầng | 23M |
| Sprint 2 | Tuần 3-4 | Quản Lý Khóa Học, Bài Học | 20M |
| Sprint 3 | Tuần 5-6 | Flashcard, Bài Kiểm Tra (Phần 1) | 18M |
| Sprint 4 | Tuần 7-8 | Bài Kiểm Tra (Phần 2), Từ Điển | 14M |
| Sprint 5 | Tuần 9-10 | Kiểm Tra Ngữ Pháp, Theo Dõi Tiến Độ | 18M |
| Sprint 6 | Tuần 11-12 | Testing & QA, Deployment | 17M |

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 27/03/2026  
**Trạng thái**: Draft

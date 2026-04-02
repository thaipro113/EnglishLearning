# 6. PROJECT BACKLOG (CHI TIẾT) - PHẦN 1

## 1. Tổng Quan Project Backlog

### 1.1 Định Nghĩa
Project Backlog là danh sách chi tiết tất cả các công việc cần thực hiện để hoàn thành dự án, được sắp xếp theo ưu tiên.

### 1.2 Cấu Trúc Backlog
- **Epic**: Nhóm công việc lớn (ví dụ: Quản Lý Người Dùng)
- **User Story**: Mô tả tính năng từ góc độ người dùng
- **Task**: Công việc cụ thể cần thực hiện
- **Sub-task**: Công việc nhỏ hơn trong một task

### 1.3 Ước Lượng Effort
- **XS (Extra Small)**: 1-2 giờ
- **S (Small)**: 3-5 giờ
- **M (Medium)**: 6-10 giờ
- **L (Large)**: 11-20 giờ
- **XL (Extra Large)**: 21+ giờ

---

## 2. Epic 1: Quản Lý Người Dùng

### Epic 1.1: Xác Thực Người Dùng

#### Story 1.1.1: Đăng Ký Tài Khoản
**ID**: US-001  
**Ưu Tiên**: Cao  
**Effort**: L  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng mới có thể tạo tài khoản bằng cách cung cấp thông tin cần thiết

**Tiêu Chí Chấp Nhận**:
- [ ] Trang đăng ký được tạo
- [ ] Xác thực username (duy nhất, 3-20 ký tự)
- [ ] Xác thực email (duy nhất, hợp lệ)
- [ ] Xác thực mật khẩu (tối thiểu 8 ký tự, chứa chữ hoa, chữ thường, số, ký tự đặc biệt)
- [ ] Gửi email xác nhận
- [ ] Kích hoạt tài khoản sau xác nhận email
- [ ] Thông báo lỗi rõ ràng

**Tasks**:
- [ ] T-001: Tạo model RegisterViewModel
- [ ] T-002: Tạo AccountController.Register()
- [ ] T-003: Tạo view đăng ký
- [ ] T-004: Thêm xác thực dữ liệu
- [ ] T-005: Tích hợp gửi email
- [ ] T-006: Tạo email template xác nhận
- [ ] T-007: Viết unit tests

---

#### Story 1.1.2: Đăng Nhập
**ID**: US-002  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng đăng nhập bằng username/email và mật khẩu hoặc OAuth

**Tiêu Chí Chấp Nhận**:
- [ ] Trang đăng nhập được tạo
- [ ] Đăng nhập bằng username/email + mật khẩu
- [ ] Đăng nhập bằng Google
- [ ] Đăng nhập bằng Facebook
- [ ] Session được tạo
- [ ] Thông báo lỗi rõ ràng
- [ ] Session timeout sau 30 phút

**Tasks**:
- [ ] T-008: Tạo AccountController.Login()
- [ ] T-009: Tạo view đăng nhập
- [ ] T-010: Cấu hình OAuth Google
- [ ] T-011: Cấu hình OAuth Facebook
- [ ] T-012: Cấu hình session timeout
- [ ] T-013: Viết unit tests

---

#### Story 1.1.3: Quên Mật Khẩu
**ID**: US-003  
**Ưu Tiên**: Trung Bình  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng có thể đặt lại mật khẩu nếu quên

**Tiêu Chí Chấp Nhận**:
- [ ] Trang quên mật khẩu được tạo
- [ ] Email đặt lại mật khẩu được gửi
- [ ] Link đặt lại mật khẩu hết hạn sau 24 giờ
- [ ] Mật khẩu mới được cập nhật
- [ ] Thông báo thành công

**Tasks**:
- [ ] T-014: Tạo AccountController.ForgotPassword()
- [ ] T-015: Tạo view quên mật khẩu
- [ ] T-016: Tạo email template đặt lại mật khẩu
- [ ] T-017: Tạo token đặt lại mật khẩu
- [ ] T-018: Viết unit tests

---

### Epic 1.2: Quản Lý Hồ Sơ Cá Nhân

#### Story 1.2.1: Xem Hồ Sơ Cá Nhân
**ID**: US-004  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem thông tin cá nhân của mình

**Tiêu Chí Chấp Nhận**:
- [ ] Trang hồ sơ được tạo
- [ ] Hiển thị tất cả thông tin cá nhân
- [ ] Hiển thị ảnh đại diện
- [ ] Hiển thị thống kê tiến độ

**Tasks**:
- [ ] T-019: Tạo UserController.Profile()
- [ ] T-020: Tạo view hồ sơ
- [ ] T-021: Viết unit tests

---

#### Story 1.2.2: Chỉnh Sửa Hồ Sơ Cá Nhân
**ID**: US-005  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng chỉnh sửa thông tin cá nhân

**Tiêu Chí Chấp Nhận**:
- [ ] Chỉnh sửa tên đầy đủ
- [ ] Chỉnh sửa số điện thoại
- [ ] Chỉnh sửa ảnh đại diện
- [ ] Chỉnh sửa nghề nghiệp
- [ ] Chỉnh sửa mục tiêu học tập
- [ ] Lưu thay đổi thành công
- [ ] Thông báo thành công

**Tasks**:
- [ ] T-022: Tạo UserController.EditProfile()
- [ ] T-023: Tạo view chỉnh sửa hồ sơ
- [ ] T-024: Xử lý upload ảnh đại diện
- [ ] T-025: Xác thực dữ liệu
- [ ] T-026: Viết unit tests

---

#### Story 1.2.3: Thay Đổi Mật Khẩu
**ID**: US-006  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng thay đổi mật khẩu

**Tiêu Chí Chấp Nhận**:
- [ ] Xác thực mật khẩu cũ
- [ ] Xác thực mật khẩu mới
- [ ] Mật khẩu mới phải khác mật khẩu cũ
- [ ] Mật khẩu được mã hóa bằng BCrypt
- [ ] Thông báo thành công

**Tasks**:
- [ ] T-027: Tạo UserController.ChangePassword()
- [ ] T-028: Tạo view thay đổi mật khẩu
- [ ] T-029: Xác thực mật khẩu
- [ ] T-030: Viết unit tests

---

## 3. Epic 2: Quản Lý Khóa Học

### Epic 2.1: Xem Khóa Học

#### Story 2.1.1: Xem Danh Sách Khóa Học
**ID**: US-007  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem danh sách các khóa học có sẵn

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị danh sách khóa học (12 khóa học/trang)
- [ ] Lọc theo cấp độ
- [ ] Tìm kiếm theo tên
- [ ] Phân trang hoạt động
- [ ] Tải trong < 2 giây

**Tasks**:
- [ ] T-031: Tạo CourseController.Index()
- [ ] T-032: Tạo view danh sách khóa học
- [ ] T-033: Thêm lọc theo cấp độ
- [ ] T-034: Thêm tìm kiếm
- [ ] T-035: Thêm phân trang
- [ ] T-036: Viết unit tests

---

#### Story 2.1.2: Xem Chi Tiết Khóa Học
**ID**: US-008  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem thông tin chi tiết của khóa học

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị tên khóa học
- [ ] Hiển thị mô tả chi tiết
- [ ] Hiển thị cấp độ
- [ ] Hiển thị danh sách bài học
- [ ] Hiển thị tiến độ (nếu đã tham gia)
- [ ] Hiển thị số người đã tham gia

**Tasks**:
- [ ] T-037: Tạo CourseController.Details()
- [ ] T-038: Tạo view chi tiết khóa học
- [ ] T-039: Viết unit tests

---

#### Story 2.1.3: Tham Gia Khóa Học
**ID**: US-009  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng tham gia một khóa học

**Tiêu Chí Chấp Nhận**:
- [ ] Người dùng có thể tham gia khóa học
- [ ] Tiến độ được tạo
- [ ] Khóa học xuất hiện trong "Khóa Học Của Tôi"
- [ ] Thông báo thành công

**Tasks**:
- [ ] T-040: Tạo CourseController.Enroll()
- [ ] T-041: Tạo bản ghi Progress
- [ ] T-042: Viết unit tests

---

### Epic 2.2: Quản Lý Khóa Học (Admin)

#### Story 2.2.1: Tạo Khóa Học
**ID**: US-010  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin tạo khóa học mới

**Tiêu Chí Chấp Nhận**:
- [ ] Trang tạo khóa học được tạo
- [ ] Nhập mã khóa học (duy nhất)
- [ ] Nhập tên khóa học
- [ ] Nhập mô tả
- [ ] Chọn cấp độ
- [ ] Khóa học được tạo thành công

**Tasks**:
- [ ] T-043: Tạo Admin/CourseController.Create()
- [ ] T-044: Tạo view tạo khóa học
- [ ] T-045: Xác thực dữ liệu
- [ ] T-046: Viết unit tests

---

#### Story 2.2.2: Chỉnh Sửa Khóa Học
**ID**: US-011  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin chỉnh sửa thông tin khóa học

**Tiêu Chí Chấp Nhận**:
- [ ] Chỉnh sửa tên, mô tả, cấp độ
- [ ] Không thể thay đổi mã khóa học
- [ ] Thay đổi được lưu ngay lập tức

**Tasks**:
- [ ] T-047: Tạo Admin/CourseController.Edit()
- [ ] T-048: Tạo view chỉnh sửa khóa học
- [ ] T-049: Viết unit tests

---

#### Story 2.2.3: Xóa Khóa Học
**ID**: US-012  
**Ưu Tiên**: Trung Bình  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin xóa khóa học

**Tiêu Chí Chấp Nhận**:
- [ ] Yêu cầu xác nhận trước khi xóa
- [ ] Khóa học được xóa hoàn toàn
- [ ] Các bài học liên quan cũng bị xóa

**Tasks**:
- [ ] T-050: Tạo Admin/CourseController.Delete()
- [ ] T-051: Tạo view xác nhận xóa
- [ ] T-052: Viết unit tests

---

## 4. Epic 3: Quản Lý Bài Học

### Epic 3.1: Xem Bài Học

#### Story 3.1.1: Xem Bài Học
**ID**: US-013  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng xem nội dung bài học

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị tiêu đề bài học
- [ ] Hiển thị nội dung bài học (hỗ trợ Markdown)
- [ ] Hiển thị hình ảnh
- [ ] Hiển thị danh sách bài học
- [ ] Có nút "Tiếp theo" và "Quay lại"
- [ ] Có nút "Đánh dấu hoàn thành"
- [ ] Hiển thị tiến độ (X/Y bài học)

**Tasks**:
- [ ] T-053: Tạo LessonsController.View()
- [ ] T-054: Tạo view xem bài học
- [ ] T-055: Tích hợp Markdown parser
- [ ] T-056: Thêm điều hướng bài học
- [ ] T-057: Thêm đánh dấu hoàn thành
- [ ] T-058: Viết unit tests

---

### Epic 3.2: Quản Lý Bài Học (Admin)

#### Story 3.2.1: Tạo Bài Học
**ID**: US-014  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin tạo bài học mới

**Tiêu Chí Chấp Nhận**:
- [ ] Chọn khóa học
- [ ] Nhập tiêu đề bài học
- [ ] Nhập nội dung (hỗ trợ Markdown)
- [ ] Upload hình ảnh
- [ ] Chọn thứ tự bài học
- [ ] Bài học được tạo thành công

**Tasks**:
- [ ] T-059: Tạo Admin/LessonController.Create()
- [ ] T-060: Tạo view tạo bài học
- [ ] T-061: Xử lý upload hình ảnh
- [ ] T-062: Xác thực Markdown
- [ ] T-063: Viết unit tests

---

#### Story 3.2.2: Chỉnh Sửa Bài Học
**ID**: US-015  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin chỉnh sửa nội dung bài học

**Tiêu Chí Chấp Nhận**:
- [ ] Chỉnh sửa tiêu đề, nội dung, hình ảnh
- [ ] Thay đổi được lưu ngay lập tức

**Tasks**:
- [ ] T-064: Tạo Admin/LessonController.Edit()
- [ ] T-065: Tạo view chỉnh sửa bài học
- [ ] T-066: Viết unit tests

---

#### Story 3.2.3: Xóa Bài Học
**ID**: US-016  
**Ưu Tiên**: Trung Bình  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin xóa bài học

**Tiêu Chí Chấp Nhận**:
- [ ] Yêu cầu xác nhận trước khi xóa
- [ ] Bài học được xóa hoàn toàn

**Tasks**:
- [ ] T-067: Tạo Admin/LessonController.Delete()
- [ ] T-068: Tạo view xác nhận xóa
- [ ] T-069: Viết unit tests

---

## 5. Epic 4: Flashcard

### Epic 4.1: Học Flashcard

#### Story 4.1.1: Học Flashcard
**ID**: US-017  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Người dùng học từ vựng bằng flashcard

**Tiêu Chí Chấp Nhận**:
- [ ] Hiển thị danh sách bộ flashcard
- [ ] Hiển thị câu hỏi
- [ ] Lật thẻ để xem đáp án
- [ ] Đánh dấu flashcard khó
- [ ] Chuyển sang flashcard tiếp theo
- [ ] Hiển thị tiến độ (X/Y flashcard)

**Tasks**:
- [ ] T-070: Tạo FlashcardController.Learn()
- [ ] T-071: Tạo view học flashcard
- [ ] T-072: Thêm chức năng lật thẻ
- [ ] T-073: Thêm đánh dấu khó
- [ ] T-074: Viết unit tests

---

### Epic 4.2: Quản Lý Flashcard (Admin)

#### Story 4.2.1: Tạo Bộ Flashcard
**ID**: US-018  
**Ưu Tiên**: Cao  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin tạo bộ flashcard mới

**Tiêu Chí Chấp Nhận**:
- [ ] Nhập tên bộ flashcard
- [ ] Nhập mô tả
- [ ] Chọn khóa học
- [ ] Bộ flashcard được tạo thành công

**Tasks**:
- [ ] T-075: Tạo Admin/FlashcardController.CreateSet()
- [ ] T-076: Tạo view tạo bộ flashcard
- [ ] T-077: Viết unit tests

---

#### Story 4.2.2: Thêm Flashcard
**ID**: US-019  
**Ưu Tiên**: Cao  
**Effort**: S  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin thêm flashcard vào bộ

**Tiêu Chí Chấp Nhận**:
- [ ] Nhập câu hỏi
- [ ] Nhập đáp án
- [ ] Flashcard được thêm thành công

**Tasks**:
- [ ] T-078: Tạo Admin/FlashcardController.AddFlashcard()
- [ ] T-079: Tạo view thêm flashcard
- [ ] T-080: Viết unit tests

---

#### Story 4.2.3: Import Flashcard từ CSV
**ID**: US-020  
**Ưu Tiên**: Trung Bình  
**Effort**: M  
**Trạng Thái**: Chưa Bắt Đầu

**Mô Tả**: Admin import flashcard từ file CSV

**Tiêu Chí Chấp Nhận**:
- [ ] Upload file CSV
- [ ] Parse CSV
- [ ] Tạo flashcard từ dữ liệu
- [ ] Thông báo thành công

**Tasks**:
- [ ] T-081: Tạo Admin/FlashcardController.ImportCSV()
- [ ] T-082: Tạo CSV parser
- [ ] T-083: Tạo view import
- [ ] T-084: Viết unit tests

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 27/03/2026  
**Trạng thái**: Draft

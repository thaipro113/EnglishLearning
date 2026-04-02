# 5. TÀI LIỆU USE CASE

## 1. Tổng Quan Use Case

### 1.1 Danh Sách Use Case Chính

| ID | Use Case | Actor | Ưu Tiên |
|----|---------|----|---------|
| UC-001 | Đăng Ký Tài Khoản | Người Dùng Mới | Cao |
| UC-002 | Đăng Nhập | Người Dùng | Cao |
| UC-003 | Quên Mật Khẩu | Người Dùng | Trung Bình |
| UC-004 | Xem Hồ Sơ Cá Nhân | Người Dùng | Cao |
| UC-005 | Chỉnh Sửa Hồ Sơ | Người Dùng | Cao |
| UC-006 | Xem Danh Sách Khóa Học | Người Dùng | Cao |
| UC-007 | Tham Gia Khóa Học | Người Dùng | Cao |
| UC-008 | Xem Bài Học | Người Dùng | Cao |
| UC-009 | Học Flashcard | Người Dùng | Cao |
| UC-010 | Làm Bài Kiểm Tra | Người Dùng | Cao |
| UC-011 | Tra Cứu Từ Điển | Người Dùng | Trung Bình |
| UC-012 | Kiểm Tra Ngữ Pháp | Người Dùng | Trung Bình |
| UC-013 | Xem Tiến Độ | Người Dùng | Cao |
| UC-014 | Tạo Khóa Học | Admin | Cao |
| UC-015 | Chỉnh Sửa Khóa Học | Admin | Cao |
| UC-016 | Xóa Khóa Học | Admin | Trung Bình |
| UC-017 | Tạo Bài Học | Admin | Cao |
| UC-018 | Chỉnh Sửa Bài Học | Admin | Cao |
| UC-019 | Xóa Bài Học | Admin | Trung Bình |
| UC-020 | Tạo Bài Kiểm Tra | Admin | Cao |
| UC-021 | Xem Báo Cáo Tiến Độ | Admin | Cao |

---

## 2. Chi Tiết Use Case

### UC-001: Đăng Ký Tài Khoản

**Mô tả**: Người dùng mới tạo tài khoản mới trên hệ thống

**Actor**: Người Dùng Mới

**Điều Kiện Tiên Quyết**:
- Người dùng chưa có tài khoản
- Người dùng có email hợp lệ

**Luồng Chính**:
1. Người dùng truy cập trang đăng ký
2. Người dùng nhập username
3. Người dùng nhập email
4. Người dùng nhập mật khẩu
5. Người dùng xác nhận mật khẩu
6. Người dùng nhập tên đầy đủ
7. Người dùng nhập số điện thoại
8. Người dùng chọn nghề nghiệp
9. Người dùng chọn mục tiêu học tập
10. Người dùng chọn cấp độ ban đầu
11. Người dùng nhấn "Đăng Ký"
12. Hệ thống xác thực dữ liệu
13. Hệ thống tạo tài khoản
14. Hệ thống gửi email xác nhận
15. Hiển thị thông báo thành công

**Luồng Ngoại Lệ**:
- **E1**: Username đã tồn tại → Hiển thị lỗi, yêu cầu nhập username khác
- **E2**: Email đã tồn tại → Hiển thị lỗi, yêu cầu nhập email khác
- **E3**: Mật khẩu không đủ mạnh → Hiển thị lỗi, yêu cầu mật khẩu mạnh hơn
- **E4**: Mật khẩu không khớp → Hiển thị lỗi, yêu cầu xác nhận lại
- **E5**: Email không hợp lệ → Hiển thị lỗi, yêu cầu email hợp lệ

**Điều Kiện Hậu Quyết**:
- Tài khoản được tạo thành công
- Email xác nhận được gửi
- Người dùng có thể đăng nhập sau khi xác nhận email

---

### UC-002: Đăng Nhập

**Mô tả**: Người dùng đăng nhập vào hệ thống

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng có tài khoản
- Tài khoản đã được kích hoạt

**Luồng Chính**:
1. Người dùng truy cập trang đăng nhập
2. Người dùng nhập username hoặc email
3. Người dùng nhập mật khẩu
4. Người dùng nhấn "Đăng Nhập"
5. Hệ thống xác thực thông tin
6. Hệ thống tạo session
7. Hiển thị trang chủ

**Luồng Ngoại Lệ**:
- **E1**: Username/Email không tồn tại → Hiển thị lỗi "Tài khoản không tồn tại"
- **E2**: Mật khẩu sai → Hiển thị lỗi "Mật khẩu không chính xác"
- **E3**: Tài khoản chưa được kích hoạt → Hiển thị lỗi "Vui lòng xác nhận email"

**Điều Kiện Hậu Quyết**:
- Người dùng được đăng nhập thành công
- Session được tạo
- Người dùng có thể truy cập các tính năng

---

### UC-003: Quên Mật Khẩu

**Mô tả**: Người dùng đặt lại mật khẩu nếu quên

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng có tài khoản
- Người dùng nhớ email đăng ký

**Luồng Chính**:
1. Người dùng truy cập trang "Quên Mật Khẩu"
2. Người dùng nhập email
3. Người dùng nhấn "Gửi"
4. Hệ thống kiểm tra email
5. Hệ thống gửi email đặt lại mật khẩu
6. Hiển thị thông báo "Email đã được gửi"
7. Người dùng nhấp vào link trong email
8. Người dùng nhập mật khẩu mới
9. Người dùng xác nhận mật khẩu mới
10. Người dùng nhấn "Đặt Lại"
11. Hệ thống cập nhật mật khẩu
12. Hiển thị thông báo thành công

**Luồng Ngoại Lệ**:
- **E1**: Email không tồn tại → Hiển thị lỗi "Email không tồn tại"
- **E2**: Link hết hạn → Hiển thị lỗi "Link đã hết hạn, vui lòng thử lại"
- **E3**: Mật khẩu mới không đủ mạnh → Hiển thị lỗi, yêu cầu mật khẩu mạnh hơn

**Điều Kiện Hậu Quyết**:
- Mật khẩu được đặt lại thành công
- Người dùng có thể đăng nhập với mật khẩu mới

---

### UC-006: Xem Danh Sách Khóa Học

**Mô tả**: Người dùng xem danh sách các khóa học có sẵn

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng đã đăng nhập
- Có ít nhất một khóa học trong hệ thống

**Luồng Chính**:
1. Người dùng truy cập trang "Khóa Học"
2. Hệ thống tải danh sách khóa học
3. Hiển thị danh sách khóa học (12 khóa học/trang)
4. Người dùng có thể lọc theo cấp độ
5. Người dùng có thể tìm kiếm theo tên
6. Người dùng có thể xem chi tiết khóa học
7. Người dùng có thể tham gia khóa học

**Luồng Ngoại Lệ**:
- **E1**: Không có khóa học → Hiển thị thông báo "Không có khóa học nào"
- **E2**: Lỗi tải dữ liệu → Hiển thị lỗi, yêu cầu thử lại

**Điều Kiện Hậu Quyết**:
- Danh sách khóa học được hiển thị
- Người dùng có thể tương tác với khóa học

---

### UC-007: Tham Gia Khóa Học

**Mô tả**: Người dùng tham gia một khóa học

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng đã đăng nhập
- Khóa học tồn tại
- Người dùng chưa tham gia khóa học này

**Luồng Chính**:
1. Người dùng xem chi tiết khóa học
2. Người dùng nhấn "Tham Gia"
3. Hệ thống ghi nhận người dùng vào khóa học
4. Hệ thống tạo bản ghi tiến độ
5. Hiển thị thông báo thành công
6. Khóa học xuất hiện trong "Khóa Học Của Tôi"

**Luồng Ngoại Lệ**:
- **E1**: Người dùng đã tham gia → Hiển thị lỗi "Bạn đã tham gia khóa học này"
- **E2**: Lỗi hệ thống → Hiển thị lỗi, yêu cầu thử lại

**Điều Kiện Hậu Quyết**:
- Người dùng được thêm vào khóa học
- Tiến độ được tạo
- Người dùng có thể bắt đầu học

---

### UC-008: Xem Bài Học

**Mô tả**: Người dùng xem nội dung bài học

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng đã đăng nhập
- Người dùng đã tham gia khóa học
- Bài học tồn tại

**Luồng Chính**:
1. Người dùng truy cập khóa học
2. Hệ thống hiển thị danh sách bài học
3. Người dùng chọn bài học
4. Hệ thống tải nội dung bài học
5. Hiển thị tiêu đề, nội dung, hình ảnh
6. Hiển thị tiến độ (X/Y bài học)
7. Người dùng có thể xem bài học trước/sau
8. Người dùng có thể đánh dấu hoàn thành

**Luồng Ngoại Lệ**:
- **E1**: Bài học không tồn tại → Hiển thị lỗi "Bài học không tồn tại"
- **E2**: Lỗi tải nội dung → Hiển thị lỗi, yêu cầu thử lại

**Điều Kiện Hậu Quyết**:
- Nội dung bài học được hiển thị
- Tiến độ được cập nhật nếu đánh dấu hoàn thành

---

### UC-009: Học Flashcard

**Mô tả**: Người dùng học từ vựng bằng flashcard

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng đã đăng nhập
- Có ít nhất một bộ flashcard

**Luồng Chính**:
1. Người dùng truy cập trang "Flashcard"
2. Hệ thống hiển thị danh sách bộ flashcard
3. Người dùng chọn bộ flashcard
4. Hệ thống tải flashcard
5. Hiển thị câu hỏi
6. Người dùng nhấn để xem đáp án
7. Hiển thị đáp án
8. Người dùng có thể đánh dấu "Khó"
9. Người dùng chuyển sang flashcard tiếp theo
10. Hiển thị tiến độ (X/Y flashcard)

**Luồng Ngoại Lệ**:
- **E1**: Không có flashcard → Hiển thị thông báo "Không có flashcard nào"
- **E2**: Lỗi tải dữ liệu → Hiển thị lỗi, yêu cầu thử lại

**Điều Kiện Hậu Quyết**:
- Tiến độ flashcard được cập nhật
- Flashcard khó được đánh dấu

---

### UC-010: Làm Bài Kiểm Tra

**Mô tả**: Người dùng làm bài kiểm tra để đánh giá kiến thức

**Actor**: Người Dùng

**Điều Kiện Tiên Quyết**:
- Người dùng đã đăng nhập
- Có ít nhất một bài kiểm tra

**Luồng Chính**:
1. Người dùng truy cập trang "Bài Kiểm Tra"
2. Hệ thống hiển thị danh sách bài kiểm tra
3. Người dùng chọn bài kiểm tra
4. Hệ thống tải câu hỏi
5. Hiển thị câu hỏi và các lựa chọn
6. Người dùng chọn đáp án
7. Người dùng chuyển sang câu hỏi tiếp theo
8. Sau khi hoàn thành, người dùng nhấn "Nộp"
9. Hệ thống tính điểm
10. Hiển thị kết quả (điểm, phần trăm đúng)
11. Hiển thị lịch sử bài kiểm tra

**Luồng Ngoại Lệ**:
- **E1**: Hết thời gian → Hệ thống tự động nộp bài
- **E2**: Mất kết nối → Hiển thị lỗi, cho phép tiếp tục

**Điều Kiện Hậu Quyết**:
- Kết quả bài kiểm tra được lưu
- Tiến độ được cập nhật
- Người dùng có thể xem lịch sử

---

### UC-014: Tạo Khóa Học (Admin)

**Mô tả**: Admin tạo khóa học mới

**Actor**: Admin

**Điều Kiện Tiên Quyết**:
- Admin đã đăng nhập
- Admin có quyền tạo khóa học

**Luồng Chính**:
1. Admin truy cập trang "Quản Lý Khóa Học"
2. Admin nhấn "Tạo Khóa Học Mới"
3. Admin nhập mã khóa học
4. Admin nhập tên khóa học
5. Admin nhập mô tả
6. Admin chọn cấp độ
7. Admin nhấn "Tạo"
8. Hệ thống xác thực dữ liệu
9. Hệ thống tạo khóa học
10. Hiển thị thông báo thành công

**Luồng Ngoại Lệ**:
- **E1**: Mã khóa học đã tồn tại → Hiển thị lỗi, yêu cầu mã khác
- **E2**: Dữ liệu không hợp lệ → Hiển thị lỗi, yêu cầu nhập lại

**Điều Kiện Hậu Quyết**:
- Khóa học được tạo thành công
- Admin có thể thêm bài học vào khóa học

---

### UC-021: Xem Báo Cáo Tiến Độ (Admin)

**Mô tả**: Admin xem báo cáo tiến độ học tập của người dùng

**Actor**: Admin

**Điều Kiện Tiên Quyết**:
- Admin đã đăng nhập
- Admin có quyền xem báo cáo

**Luồng Chính**:
1. Admin truy cập trang "Báo Cáo"
2. Hệ thống tải dữ liệu báo cáo
3. Hiển thị thống kê tổng quát (tổng người dùng, khóa học, bài kiểm tra)
4. Admin có thể lọc theo thời gian
5. Admin có thể lọc theo khóa học
6. Admin có thể xem tiến độ từng người dùng
7. Admin có thể xuất báo cáo (PDF/Excel)

**Luồng Ngoại Lệ**:
- **E1**: Lỗi tải dữ liệu → Hiển thị lỗi, yêu cầu thử lại
- **E2**: Lỗi xuất báo cáo → Hiển thị lỗi, yêu cầu thử lại

**Điều Kiện Hậu Quyết**:
- Báo cáo được hiển thị chính xác
- Admin có thể xuất báo cáo

---

## 3. Sơ Đồ Use Case

```
                    ┌─────────────────────────────────┐
                    │      Người Dùng Mới             │
                    └────────────┬────────────────────┘
                                 │
                                 │ Đăng Ký
                                 ↓
                    ┌─────────────────────────────────┐
                    │   Hệ Thống Học Tiếng Anh        │
                    │                                 │
                    │  ┌─────────────────────────┐   │
                    │  │  UC-001: Đăng Ký       │   │
                    │  │  UC-002: Đăng Nhập     │   │
                    │  │  UC-003: Quên MK       │   │
                    │  │  UC-004: Xem Hồ Sơ    │   │
                    │  │  UC-005: Chỉnh Sửa    │   │
                    │  │  UC-006: Xem Khóa Học │   │
                    │  │  UC-007: Tham Gia     │   │
                    │  │  UC-008: Xem Bài Học  │   │
                    │  │  UC-009: Flashcard    │   │
                    │  │  UC-010: Bài Kiểm Tra │   │
                    │  │  UC-011: Từ Điển      │   │
                    │  │  UC-012: Ngữ Pháp     │   │
                    │  │  UC-013: Tiến Độ      │   │
                    │  └─────────────────────────┘   │
                    │                                 │
                    │  ┌─────────────────────────┐   │
                    │  │  UC-014: Tạo Khóa Học  │   │
                    │  │  UC-015: Chỉnh Sửa     │   │
                    │  │  UC-016: Xóa           │   │
                    │  │  UC-017: Tạo Bài Học  │   │
                    │  │  UC-018: Chỉnh Sửa     │   │
                    │  │  UC-019: Xóa           │   │
                    │  │  UC-020: Tạo Quiz      │   │
                    │  │  UC-021: Báo Cáo       │   │
                    │  └─────────────────────────┘   │
                    │                                 │
                    └─────────────────────────────────┘
                                 ↑
                    ┌────────────┴────────────────────┐
                    │                                 │
        ┌───────────────────────┐      ┌──────────────────────┐
        │   Người Dùng Thường   │      │      Admin           │
        └───────────────────────┘      └──────────────────────┘
```

---

## 4. Bảng Tóm Tắt Use Case

| UC ID | Tên | Actor | Độ Phức Tạp | Ưu Tiên |
|-------|-----|-------|------------|---------|
| UC-001 | Đăng Ký | Người Dùng Mới | Trung Bình | Cao |
| UC-002 | Đăng Nhập | Người Dùng | Thấp | Cao |
| UC-003 | Quên MK | Người Dùng | Trung Bình | Trung Bình |
| UC-004 | Xem Hồ Sơ | Người Dùng | Thấp | Cao |
| UC-005 | Chỉnh Sửa Hồ Sơ | Người Dùng | Thấp | Cao |
| UC-006 | Xem Khóa Học | Người Dùng | Thấp | Cao |
| UC-007 | Tham Gia | Người Dùng | Thấp | Cao |
| UC-008 | Xem Bài Học | Người Dùng | Thấp | Cao |
| UC-009 | Flashcard | Người Dùng | Trung Bình | Cao |
| UC-010 | Bài Kiểm Tra | Người Dùng | Cao | Cao |
| UC-011 | Từ Điển | Người Dùng | Trung Bình | Trung Bình |
| UC-012 | Ngữ Pháp | Người Dùng | Cao | Trung Bình |
| UC-013 | Tiến Độ | Người Dùng | Trung Bình | Cao |
| UC-014 | Tạo Khóa Học | Admin | Trung Bình | Cao |
| UC-015 | Chỉnh Sửa Khóa Học | Admin | Trung Bình | Cao |
| UC-016 | Xóa Khóa Học | Admin | Thấp | Trung Bình |
| UC-017 | Tạo Bài Học | Admin | Trung Bình | Cao |
| UC-018 | Chỉnh Sửa Bài Học | Admin | Trung Bình | Cao |
| UC-019 | Xóa Bài Học | Admin | Thấp | Trung Bình |
| UC-020 | Tạo Bài Kiểm Tra | Admin | Cao | Cao |
| UC-021 | Báo Cáo | Admin | Cao | Cao |

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 27/03/2026  
**Trạng thái**: Draft

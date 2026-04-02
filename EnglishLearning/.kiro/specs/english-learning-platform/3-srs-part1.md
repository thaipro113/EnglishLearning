# 3. SOFTWARE REQUIREMENTS SPECIFICATION (SRS) - PHẦN 1

## 1. Giới Thiệu

### 1.1 Mục Đích Tài Liệu
Tài liệu này mô tả chi tiết các yêu cầu kỹ thuật cho nền tảng học tiếng Anh trực tuyến, bao gồm các chức năng, giao diện, cơ sở dữ liệu, và yêu cầu phi chức năng.

### 1.2 Phạm Vi
Tài liệu này bao gồm:
- Các yêu cầu chức năng chi tiết
- Các yêu cầu phi chức năng
- Mô tả giao diện người dùng
- Mô tả cơ sở dữ liệu
- Yêu cầu bảo mật
- Yêu cầu hiệu năng

### 1.3 Định Nghĩa và Viết Tắt

| Viết Tắt | Ý Nghĩa |
|----------|---------|
| API | Application Programming Interface |
| CRUD | Create, Read, Update, Delete |
| UI | User Interface |
| UX | User Experience |
| DB | Database |
| JWT | JSON Web Token |
| OAuth | Open Authorization |
| TOEIC | Test of English for International Communication |
| EF Core | Entity Framework Core |
| MVC | Model-View-Controller |
| HTTPS | HyperText Transfer Protocol Secure |
| CSRF | Cross-Site Request Forgery |
| XSS | Cross-Site Scripting |
| SQL | Structured Query Language |

---

## 2. Yêu Cầu Chức Năng

### 2.1 Quản Lý Người Dùng

#### 2.1.1 Đăng Ký Người Dùng

**ID**: REQ-USER-001  
**Tiêu đề**: Đăng ký tài khoản mới  
**Mô tả**: Người dùng mới có thể tạo tài khoản bằng cách cung cấp thông tin cần thiết

**Chi tiết yêu cầu**:
- Người dùng nhập username (3-20 ký tự, chỉ chứa chữ cái, số, dấu gạch dưới)
- Người dùng nhập email hợp lệ
- Người dùng nhập mật khẩu (tối thiểu 8 ký tự, chứa chữ hoa, chữ thường, số, ký tự đặc biệt)
- Người dùng xác nhận mật khẩu
- Người dùng nhập tên đầy đủ
- Người dùng nhập số điện thoại (định dạng Việt Nam)
- Người dùng chọn nghề nghiệp
- Người dùng chọn mục tiêu học tập
- Người dùng chọn cấp độ ban đầu
- Hệ thống gửi email xác nhận
- Người dùng xác nhận email
- Tài khoản được kích hoạt

**Tiêu chí chấp nhận**:
- Username phải duy nhất trong hệ thống
- Email phải duy nhất và hợp lệ
- Mật khẩu phải đáp ứng yêu cầu độ mạnh
- Email xác nhận được gửi trong vòng 1 phút
- Tài khoản được kích hoạt sau khi xác nhận email
- Thông báo lỗi rõ ràng nếu dữ liệu không hợp lệ

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.1.2 Đăng Nhập

**ID**: REQ-USER-002  
**Tiêu đề**: Đăng nhập vào hệ thống  
**Mô tả**: Người dùng đăng nhập bằng username/email và mật khẩu hoặc OAuth

**Chi tiết yêu cầu**:
- Người dùng nhập username hoặc email
- Người dùng nhập mật khẩu
- Hệ thống xác thực thông tin
- Nếu thành công, tạo session/token
- Nếu thất bại, hiển thị thông báo lỗi
- Hỗ trợ đăng nhập qua Google
- Hỗ trợ đăng nhập qua Facebook

**Tiêu chí chấp nhận**:
- Đăng nhập thành công với thông tin chính xác
- Thông báo lỗi rõ ràng khi thông tin sai
- Session timeout sau 30 phút không hoạt động
- OAuth redirect đúng
- Lưu thông tin đăng nhập an toàn

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.1.3 Quản Lý Hồ Sơ Cá Nhân

**ID**: REQ-USER-003  
**Tiêu đề**: Xem và chỉnh sửa hồ sơ cá nhân  
**Mô tả**: Người dùng có thể xem và cập nhật thông tin cá nhân

**Chi tiết yêu cầu**:
- Người dùng xem thông tin cá nhân (username, email, tên, số điện thoại, ảnh đại diện)
- Người dùng chỉnh sửa tên đầy đủ
- Người dùng chỉnh sửa số điện thoại
- Người dùng chỉnh sửa ảnh đại diện
- Người dùng chỉnh sửa nghề nghiệp
- Người dùng chỉnh sửa mục tiêu học tập
- Hệ thống lưu thay đổi
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Cập nhật thông tin thành công
- Ảnh đại diện tối đa 5MB, định dạng JPG/PNG
- Thay đổi được lưu ngay lập tức
- Hiển thị thông báo xác nhận

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.1.4 Thay Đổi Mật Khẩu

**ID**: REQ-USER-004  
**Tiêu đề**: Thay đổi mật khẩu  
**Mô tả**: Người dùng có thể thay đổi mật khẩu của mình

**Chi tiết yêu cầu**:
- Người dùng nhập mật khẩu cũ
- Hệ thống xác thực mật khẩu cũ
- Người dùng nhập mật khẩu mới
- Người dùng xác nhận mật khẩu mới
- Hệ thống kiểm tra độ mạnh của mật khẩu
- Hệ thống lưu mật khẩu mới (mã hóa)
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Mật khẩu cũ phải chính xác
- Mật khẩu mới phải khác mật khẩu cũ
- Mật khẩu mới phải đáp ứng yêu cầu độ mạnh
- Mật khẩu được mã hóa bằng BCrypt

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.1.5 Quên Mật Khẩu

**ID**: REQ-USER-005  
**Tiêu đề**: Đặt lại mật khẩu  
**Mô tả**: Người dùng có thể đặt lại mật khẩu nếu quên

**Chi tiết yêu cầu**:
- Người dùng nhập email
- Hệ thống gửi email đặt lại mật khẩu
- Người dùng nhấp vào link trong email
- Người dùng nhập mật khẩu mới
- Hệ thống lưu mật khẩu mới
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Email được gửi trong vòng 1 phút
- Link đặt lại mật khẩu hết hạn sau 24 giờ
- Mật khẩu mới phải đáp ứng yêu cầu độ mạnh

**Ưu tiên**: Trung bình  
**Trạng thái**: Bắt buộc

---

### 2.2 Quản Lý Khóa Học

#### 2.2.1 Xem Danh Sách Khóa Học

**ID**: REQ-COURSE-001  
**Tiêu đề**: Xem danh sách khóa học  
**Mô tả**: Người dùng xem các khóa học có sẵn

**Chi tiết yêu cầu**:
- Hiển thị danh sách khóa học
- Lọc theo cấp độ (Beginner, Intermediate, Advanced)
- Tìm kiếm theo tên khóa học
- Xem mô tả khóa học
- Xem số bài học trong khóa học
- Xem tiến độ của khóa học đã tham gia
- Phân trang (12 khóa học/trang)

**Tiêu chí chấp nhận**:
- Danh sách tải trong < 2 giây
- Tìm kiếm hoạt động chính xác
- Lọc theo cấp độ hoạt động
- Phân trang hoạt động

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.2.2 Tham Gia Khóa Học

**ID**: REQ-COURSE-002  
**Tiêu đề**: Tham gia khóa học  
**Mô tả**: Người dùng tham gia một khóa học

**Chi tiết yêu cầu**:
- Người dùng chọn khóa học
- Người dùng nhấn "Tham gia"
- Hệ thống ghi nhận người dùng vào khóa học
- Hiển thị thông báo thành công
- Khóa học xuất hiện trong "Khóa học của tôi"

**Tiêu chí chấp nhận**:
- Người dùng có thể tham gia nhiều khóa học
- Tiến độ được lưu lại
- Có thể rời khóa học bất kỳ lúc nào

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.2.3 Xem Chi Tiết Khóa Học

**ID**: REQ-COURSE-003  
**Tiêu đề**: Xem chi tiết khóa học  
**Mô tả**: Người dùng xem thông tin chi tiết của khóa học

**Chi tiết yêu cầu**:
- Hiển thị tên khóa học
- Hiển thị mô tả chi tiết
- Hiển thị cấp độ
- Hiển thị danh sách bài học
- Hiển thị tiến độ (nếu đã tham gia)
- Hiển thị số người đã tham gia

**Tiêu chí chấp nhận**:
- Thông tin hiển thị chính xác
- Danh sách bài học sắp xếp đúng thứ tự

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.2.4 Quản Lý Khóa Học (Admin)

**ID**: REQ-COURSE-ADMIN-001  
**Tiêu đề**: Tạo khóa học mới  
**Mô tả**: Admin tạo khóa học mới

**Chi tiết yêu cầu**:
- Admin nhập mã khóa học (duy nhất)
- Admin nhập tên khóa học
- Admin nhập mô tả
- Admin chọn cấp độ
- Admin nhấn "Tạo"
- Hệ thống lưu khóa học
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Mã khóa học phải duy nhất
- Tên khóa học bắt buộc
- Cấp độ phải được chọn
- Khóa học được tạo thành công

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

**ID**: REQ-COURSE-ADMIN-002  
**Tiêu đề**: Chỉnh sửa khóa học  
**Mô tả**: Admin chỉnh sửa thông tin khóa học

**Chi tiết yêu cầu**:
- Admin chọn khóa học
- Admin chỉnh sửa tên, mô tả, cấp độ
- Admin nhấn "Lưu"
- Hệ thống cập nhật khóa học
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Thay đổi được lưu ngay lập tức
- Không thể thay đổi mã khóa học

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

**ID**: REQ-COURSE-ADMIN-003  
**Tiêu đề**: Xóa khóa học  
**Mô tả**: Admin xóa khóa học

**Chi tiết yêu cầu**:
- Admin chọn khóa học
- Admin nhấn "Xóa"
- Hệ thống yêu cầu xác nhận
- Admin xác nhận
- Hệ thống xóa khóa học
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Yêu cầu xác nhận trước khi xóa
- Khóa học được xóa hoàn toàn
- Các bài học liên quan cũng bị xóa

**Ưu tiên**: Trung bình  
**Trạng thái**: Bắt buộc

---

### 2.3 Quản Lý Bài Học

#### 2.3.1 Xem Bài Học

**ID**: REQ-LESSON-001  
**Tiêu đề**: Xem nội dung bài học  
**Mô tả**: Người dùng xem nội dung bài học

**Chi tiết yêu cầu**:
- Hiển thị tiêu đề bài học
- Hiển thị nội dung bài học (hỗ trợ Markdown)
- Hiển thị danh sách bài học trong khóa học
- Có nút "Tiếp theo" để đi đến bài học tiếp theo
- Có nút "Quay lại" để quay lại bài học trước
- Có nút "Đánh dấu hoàn thành"
- Hiển thị tiến độ (X/Y bài học)

**Tiêu chí chấp nhận**:
- Nội dung hiển thị đúng định dạng Markdown
- Hình ảnh hiển thị đúng
- Điều hướng hoạt động chính xác
- Tiến độ được cập nhật

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

#### 2.3.2 Quản Lý Bài Học (Admin)

**ID**: REQ-LESSON-ADMIN-001  
**Tiêu đề**: Tạo bài học mới  
**Mô tả**: Admin tạo bài học mới

**Chi tiết yêu cầu**:
- Admin chọn khóa học
- Admin nhập tiêu đề bài học
- Admin nhập nội dung (hỗ trợ Markdown)
- Admin có thể upload hình ảnh
- Admin chọn thứ tự bài học
- Admin nhấn "Tạo"
- Hệ thống lưu bài học
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Bài học được tạo thành công
- Nội dung Markdown được lưu đúng
- Hình ảnh được upload thành công
- Thứ tự bài học được cập nhật

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

**ID**: REQ-LESSON-ADMIN-002  
**Tiêu đề**: Chỉnh sửa bài học  
**Mô tả**: Admin chỉnh sửa nội dung bài học

**Chi tiết yêu cầu**:
- Admin chọn bài học
- Admin chỉnh sửa tiêu đề, nội dung, hình ảnh
- Admin nhấn "Lưu"
- Hệ thống cập nhật bài học
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Thay đổi được lưu ngay lập tức
- Nội dung Markdown được lưu đúng

**Ưu tiên**: Cao  
**Trạng thái**: Bắt buộc

---

**ID**: REQ-LESSON-ADMIN-003  
**Tiêu đề**: Xóa bài học  
**Mô tả**: Admin xóa bài học

**Chi tiết yêu cầu**:
- Admin chọn bài học
- Admin nhấn "Xóa"
- Hệ thống yêu cầu xác nhận
- Admin xác nhận
- Hệ thống xóa bài học
- Hiển thị thông báo thành công

**Tiêu chí chấp nhận**:
- Yêu cầu xác nhận trước khi xóa
- Bài học được xóa hoàn toàn

**Ưu tiên**: Trung bình  
**Trạng thái**: Bắt buộc

---

## 3. Yêu Cầu Giao Diện Người Dùng

### 3.1 Giao Diện Chung
- Responsive design (mobile, tablet, desktop)
- Màu sắc: Xanh dương chính, trắng nền
- Font: Roboto (tiêu đề), Open Sans (nội dung)
- Kích thước font: 14px (nội dung), 24px (tiêu đề)

### 3.2 Thanh Điều Hướng
- Logo ứng dụng
- Menu chính (Khóa học, Flashcard, Quiz, Từ điển, Kiểm tra ngữ pháp)
- Tìm kiếm
- Thông báo
- Hồ sơ người dùng

### 3.3 Trang Chủ
- Banner chào mừng
- Khóa học được đề xuất
- Khóa học của tôi
- Thống kê tiến độ

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 27/03/2026  
**Trạng thái**: Draft

# Hướng dẫn Đăng nhập Admin riêng biệt

## Tổng quan
Hệ thống đã được cấu hình để có 2 authentication schemes hoàn toàn tách biệt:
- **UserScheme**: Dành cho người dùng thông thường (cookie: `UserAuth`)
- **AdminScheme**: Dành cho quản trị viên (cookie: `AdminAuth`)

## Các thay đổi đã thực hiện

### 1. Program.cs
- Thêm 2 cookie authentication schemes riêng biệt
- `UserAuth` cookie cho user thông thường
- `AdminAuth` cookie cho admin

### 2. Admin Authentication Controller
**File**: `EnglishLearning/Areas/Admin/Controllers/AuthController.cs`
- Xử lý đăng nhập/đăng xuất riêng cho Admin
- Kiểm tra Role = "Admin" trước khi cho phép đăng nhập
- Sử dụng Claims-based authentication với AdminScheme

### 3. Admin Authorization Attribute
**File**: `EnglishLearning/Areas/Admin/Attributes/AdminAuthorizeAttribute.cs`
- Custom attribute để bảo vệ các controller Admin
- Kiểm tra xem user có đăng nhập với AdminScheme không
- Tự động redirect về trang login admin nếu chưa đăng nhập

### 4. Views
- **Login**: `/Admin/Auth/Login` - Trang đăng nhập admin với giao diện riêng
- **AccessDenied**: `/Admin/Auth/AccessDenied` - Trang thông báo từ chối truy cập

### 5. Cập nhật Controllers
Tất cả các controller trong Admin area đã được thêm attribute `[AdminAuthorize]`:
- HomeController
- LessonController
- CourseController
- ProgressController
- UserController (AccountController)

### 6. Layout Admin
- Thêm nút Logout sử dụng form POST
- Cập nhật link Dashboard

## Cách sử dụng

### Đăng nhập Admin
1. Truy cập: `https://yourdomain.com/Admin/Auth/Login`
2. Nhập username và password của tài khoản có Role = "Admin"
3. Hệ thống sẽ kiểm tra:
   - Username và password có đúng không
   - Role có phải là "Admin" không
4. Nếu thành công, redirect về `/Admin/Home/Index`

### Đăng xuất Admin
- Click vào dropdown menu ở góc phải trên
- Click "Logout"
- Hệ thống sẽ xóa AdminAuth cookie và redirect về trang login admin

### Bảo mật
- Admin đăng nhập sẽ KHÔNG ảnh hưởng đến session user
- User đăng nhập sẽ KHÔNG thể truy cập vào Admin area
- Mỗi scheme có cookie riêng biệt
- Khi đăng xuất Admin, session User vẫn được giữ nguyên

## Kiểm tra

### Test case 1: Đăng nhập Admin
1. Đăng nhập vào trang user bình thường
2. Mở tab mới, truy cập `/Admin/Auth/Login`
3. Đăng nhập với tài khoản admin
4. Kiểm tra: Cả 2 session phải hoạt động độc lập

### Test case 2: Truy cập không có quyền
1. Chưa đăng nhập admin
2. Truy cập `/Admin/Home/Index`
3. Kết quả: Tự động redirect về `/Admin/Auth/Login`

### Test case 3: User không phải Admin
1. Đăng nhập với tài khoản Role = "User"
2. Truy cập `/Admin/Auth/Login`
3. Nhập username/password
4. Kết quả: Hiển thị lỗi "Bạn không có quyền truy cập vào trang quản trị"

## Lưu ý kỹ thuật

### Claims được lưu trong AdminScheme
- `ClaimTypes.Name`: Username
- `ClaimTypes.NameIdentifier`: UserId
- `ClaimTypes.Role`: "Admin"
- `FullName`: Tên đầy đủ
- `ImageUrl`: Đường dẫn ảnh đại diện
- `AuthScheme`: "AdminScheme" (để phân biệt với UserScheme)

### Cookie Settings
- **UserAuth**: 
  - LoginPath: `/Account/Login`
  - AccessDeniedPath: `/Account/AccessDenied`
  
- **AdminAuth**:
  - LoginPath: `/Admin/Auth/Login`
  - AccessDeniedPath: `/Admin/Auth/AccessDenied`
  - IsPersistent: true
  - ExpiresUtc: 8 giờ

## Troubleshooting

### Vấn đề: Admin đăng nhập nhưng bị logout ngay
- Kiểm tra cookie `AdminAuth` có được tạo không
- Kiểm tra claim `AuthScheme` có giá trị "AdminScheme" không

### Vấn đề: Redirect loop
- Kiểm tra LoginPath trong Program.cs
- Đảm bảo AuthController không có attribute `[AdminAuthorize]`

### Vấn đề: User bị logout khi Admin đăng nhập
- Kiểm tra cookie name phải khác nhau
- Kiểm tra authentication scheme name phải khác nhau

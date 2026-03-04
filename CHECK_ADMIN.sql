-- Kiểm tra tài khoản Admin trong database
SELECT 
    UserId,
    Username,
    FullName,
    Email,
    Role,
    CreatedAt,
    CASE 
        WHEN PasswordHash LIKE '$2a$%' OR PasswordHash LIKE '$2b$%' THEN 'BCrypt Hash (OK)'
        ELSE 'Plain Text (ERROR - Cần hash lại!)'
    END AS PasswordStatus
FROM Users 
WHERE Role = 'Admin';

-- Nếu không có kết quả, nghĩa là chưa có admin
-- Nếu PasswordStatus = 'Plain Text', cần xóa và tạo lại

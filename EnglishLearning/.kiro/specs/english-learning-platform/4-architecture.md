# 4. KIẾN TRÚC HỆ THỐNG PHẦN MỀM

## 1. Tổng Quan Kiến Trúc

### 1.1 Mô Hình Kiến Trúc
Nền tảng học tiếng Anh sử dụng kiến trúc **MVC (Model-View-Controller)** với các lớp sau:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│              (Views - HTML/CSS/JavaScript)               │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                   Controller Layer                       │
│         (Request Handling & Business Logic)              │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                    Service Layer                         │
│              (Business Logic & Validation)               │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                 Repository Layer                         │
│              (Data Access & Persistence)                 │
└─────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────┐
│                    Data Layer                            │
│                  (SQL Server Database)                   │
└─────────────────────────────────────────────────────────┘
```

### 1.2 Công Nghệ Stack

| Lớp | Công Nghệ |
|-----|-----------|
| **Frontend** | HTML5, CSS3, Bootstrap, JavaScript |
| **Backend** | ASP.NET Core 8.0, C# |
| **Database** | SQL Server 2019+ |
| **ORM** | Entity Framework Core 9.0 |
| **Authentication** | Cookie-based + OAuth 2.0 |
| **Containerization** | Docker |
| **Web Server** | Kestrel |

---

## 2. Các Thành Phần Chính

### 2.1 Presentation Layer (Lớp Trình Bày)

#### 2.1.1 Views
- **Home Views**: Trang chủ, giới thiệu
- **Account Views**: Đăng ký, đăng nhập, quên mật khẩu
- **Course Views**: Danh sách khóa học, chi tiết khóa học
- **Lesson Views**: Xem bài học
- **Flashcard Views**: Học flashcard
- **Quiz Views**: Làm bài kiểm tra
- **Dictionary Views**: Tra cứu từ điển
- **Grammar Views**: Kiểm tra ngữ pháp
- **User Views**: Hồ sơ cá nhân, tiến độ
- **Admin Views**: Dashboard, quản lý nội dung

#### 2.1.2 Static Files
- CSS files (Bootstrap, custom styles)
- JavaScript files (jQuery, custom scripts)
- Images (logos, icons)
- Fonts (Roboto, Open Sans)

### 2.2 Controller Layer (Lớp Điều Khiển)

#### 2.2.1 Controllers
```
Controllers/
├── HomeController.cs          # Trang chủ
├── AccountController.cs       # Đăng ký, đăng nhập
├── CourseController.cs        # Quản lý khóa học
├── LessonsController.cs       # Quản lý bài học
├── FlashcardController.cs     # Quản lý flashcard
├── QuizController.cs          # Quản lý bài kiểm tra
├── DictionaryController.cs    # Quản lý từ điển
├── GrammarController.cs       # Kiểm tra ngữ pháp
├── UserController.cs          # Quản lý người dùng
├── TranslationController.cs   # Dịch thuật
└── TestController.cs          # Test

Areas/Admin/
├── Controllers/
│   ├── HomeController.cs      # Dashboard admin
│   ├── CourseController.cs    # Quản lý khóa học
│   ├── LessonController.cs    # Quản lý bài học
│   ├── FlashcardController.cs # Quản lý flashcard
│   ├── QuizController.cs      # Quản lý bài kiểm tra
│   ├── UserController.cs      # Quản lý người dùng
│   └── ReportController.cs    # Báo cáo
```

### 2.3 Service Layer (Lớp Dịch Vụ)

#### 2.3.1 Services
```
Services/
├── IUserService.cs            # Interface dịch vụ người dùng
├── UserService.cs             # Dịch vụ người dùng
├── ICourseService.cs          # Interface dịch vụ khóa học
├── CourseService.cs           # Dịch vụ khóa học
├── ILessonService.cs          # Interface dịch vụ bài học
├── LessonService.cs           # Dịch vụ bài học
├── IFlashcardService.cs       # Interface dịch vụ flashcard
├── FlashcardService.cs        # Dịch vụ flashcard
├── IQuizService.cs            # Interface dịch vụ bài kiểm tra
├── QuizService.cs             # Dịch vụ bài kiểm tra
├── IDictionaryService.cs      # Interface dịch vụ từ điển
├── DictionaryService.cs       # Dịch vụ từ điển
├── IGrammarService.cs         # Interface dịch vụ kiểm tra ngữ pháp
├── GrammarService.cs          # Dịch vụ kiểm tra ngữ pháp
├── IProgressService.cs        # Interface dịch vụ tiến độ
├── ProgressService.cs         # Dịch vụ tiến độ
├── IAuthenticationService.cs  # Interface dịch vụ xác thực
└── AuthenticationService.cs   # Dịch vụ xác thực
```

### 2.4 Repository Layer (Lớp Kho Dữ Liệu)

#### 2.4.1 Repositories
```
Repositories/
├── IRepository.cs             # Interface kho dữ liệu chung
├── Repository.cs              # Kho dữ liệu chung
├── IUserRepository.cs         # Interface kho dữ liệu người dùng
├── UserRepository.cs          # Kho dữ liệu người dùng
├── ICourseRepository.cs       # Interface kho dữ liệu khóa học
├── CourseRepository.cs        # Kho dữ liệu khóa học
├── ILessonRepository.cs       # Interface kho dữ liệu bài học
├── LessonRepository.cs        # Kho dữ liệu bài học
├── IFlashcardRepository.cs    # Interface kho dữ liệu flashcard
├── FlashcardRepository.cs     # Kho dữ liệu flashcard
├── IQuizRepository.cs         # Interface kho dữ liệu bài kiểm tra
├── QuizRepository.cs          # Kho dữ liệu bài kiểm tra
├── IDictionaryRepository.cs   # Interface kho dữ liệu từ điển
├── DictionaryRepository.cs    # Kho dữ liệu từ điển
├── IProgressRepository.cs     # Interface kho dữ liệu tiến độ
└── ProgressRepository.cs      # Kho dữ liệu tiến độ
```

### 2.5 Data Layer (Lớp Dữ Liệu)

#### 2.5.1 Models
```
Models/
├── User.cs                    # Người dùng
├── Course.cs                  # Khóa học
├── Lesson.cs                  # Bài học
├── Flashcard.cs               # Flashcard
├── FlashcardSet.cs            # Bộ flashcard
├── Quiz.cs                    # Bài kiểm tra
├── QuizQuestion.cs            # Câu hỏi bài kiểm tra
├── QuizAnswer.cs              # Đáp án bài kiểm tra
├── Progress.cs                # Tiến độ
├── TranslationHistory.cs      # Lịch sử dịch thuật
├── ChatHistory.cs             # Lịch sử chat
├── Paragraph.cs               # Đoạn văn
├── ParagraphData.cs           # Dữ liệu đoạn văn
├── LearningLevel.cs           # Cấp độ học tập
├── SubLevel.cs                # Cấp độ con
├── EnglishLearningDbContext.cs # DbContext
└── ErrorViewModel.cs          # Model lỗi
```

#### 2.5.2 Database Context
```csharp
public class EnglishLearningDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<FlashcardSet> FlashcardSets { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Progress> Progresses { get; set; }
    public DbSet<TranslationHistory> TranslationHistories { get; set; }
    // ... other DbSets
}
```

---

## 3. Sơ Đồ Kiến Trúc Chi Tiết

### 3.1 Luồng Yêu Cầu (Request Flow)

```
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │ HTTP Request
       ↓
┌─────────────────────────────────────┐
│   ASP.NET Core Middleware Pipeline  │
│  (Authentication, Authorization)    │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────────────────────────────┐
│        Routing Engine               │
│   (Matches URL to Controller)       │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────────────────────────────┐
│        Controller Action            │
│   (Processes Request)               │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────────────────────────────┐
│        Service Layer                │
│   (Business Logic)                  │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────────────────────────────┐
│        Repository Layer             │
│   (Data Access)                     │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────────────────────────────┐
│        Entity Framework Core        │
│   (ORM)                             │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────────────────────────────┐
│        SQL Server Database          │
│   (Data Persistence)                │
└──────┬──────────────────────────────┘
       │
       ↓ (Response)
┌─────────────────────────────────────┐
│        View/JSON Response           │
│   (Rendered HTML or JSON)           │
└──────┬──────────────────────────────┘
       │
       ↓
┌─────────────┐
│   Browser   │
└─────────────┘
```

### 3.2 Sơ Đồ Thực Thể - Mối Quan Hệ (Entity Relationship Diagram)

```
User (1) ──────────────── (N) Progress
  │
  ├─────────────────────── (N) Paragraph
  │
  └─────────────────────── (N) TranslationHistory

Course (1) ──────────────── (N) Lesson
  │
  └─────────────────────── (1) SubLevel

SubLevel (1) ──────────────── (N) Course
  │
  └─────────────────────── (1) LearningLevel

Lesson (1) ──────────────── (N) Progress

FlashcardSet (1) ──────────────── (N) Flashcard

Quiz (1) ──────────────── (N) QuizQuestion

QuizQuestion (1) ──────────────── (N) QuizAnswer
```

---

## 4. Các Mô-đun Chính

### 4.1 Mô-đun Xác Thực (Authentication Module)
- Đăng ký người dùng
- Đăng nhập
- OAuth (Google, Facebook)
- Quản lý session
- Đặt lại mật khẩu

### 4.2 Mô-đun Khóa Học (Course Module)
- Tạo/chỉnh sửa/xóa khóa học
- Xem danh sách khóa học
- Tham gia khóa học
- Quản lý bài học

### 4.3 Mô-đun Flashcard (Flashcard Module)
- Tạo/chỉnh sửa/xóa flashcard
- Học flashcard
- Theo dõi tiến độ flashcard

### 4.4 Mô-đun Bài Kiểm Tra (Quiz Module)
- Tạo/chỉnh sửa/xóa bài kiểm tra
- Làm bài kiểm tra
- Tính điểm
- Xem kết quả

### 4.5 Mô-đun Từ Điển (Dictionary Module)
- Tra cứu từ
- Quản lý từ vựng
- Lưu từ yêu thích

### 4.6 Mô-đun Kiểm Tra Ngữ Pháp (Grammar Module)
- Kiểm tra ngữ pháp
- Gợi ý sửa lỗi
- Giải thích lỗi

### 4.7 Mô-đun Tiến Độ (Progress Module)
- Theo dõi tiến độ khóa học
- Thống kê bài kiểm tra
- Báo cáo tiến độ

### 4.8 Mô-đun Quản Trị (Admin Module)
- Dashboard quản trị
- Quản lý người dùng
- Quản lý nội dung
- Xem báo cáo

---

## 5. Cơ Sở Dữ Liệu

### 5.1 Sơ Đồ Cơ Sở Dữ Liệu

#### Bảng Users
```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE,
    PhoneNumber NVARCHAR(10),
    Role NVARCHAR(50),
    CreatedAt DATETIME,
    Occupation NVARCHAR(MAX),
    Level NVARCHAR(MAX),
    Purpose NVARCHAR(MAX),
    ImageUrl NVARCHAR(MAX)
);
```

#### Bảng Courses
```sql
CREATE TABLE Courses (
    CourseId NVARCHAR(10) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    SubLevelId INT,
    CreatedAt DATETIME,
    FOREIGN KEY (SubLevelId) REFERENCES SubLevels(SubLevelId)
);
```

#### Bảng Lessons
```sql
CREATE TABLE Lessons (
    LessonId INT PRIMARY KEY IDENTITY(1,1),
    CourseId NVARCHAR(10) NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX),
    OrderNumber INT,
    CreatedAt DATETIME,
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId)
);
```

#### Bảng Flashcards
```sql
CREATE TABLE Flashcards (
    FlashcardId INT PRIMARY KEY IDENTITY(1,1),
    FlashcardSetId INT NOT NULL,
    Question NVARCHAR(MAX) NOT NULL,
    Answer NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME,
    FOREIGN KEY (FlashcardSetId) REFERENCES FlashcardSets(FlashcardSetId)
);
```

#### Bảng Progress
```sql
CREATE TABLE Progresses (
    ProgressId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    CourseId NVARCHAR(10),
    LessonId INT,
    CompletedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    FOREIGN KEY (LessonId) REFERENCES Lessons(LessonId)
);
```

---

## 6. Bảo Mật

### 6.1 Xác Thực (Authentication)
- Cookie-based authentication cho người dùng thường
- OAuth 2.0 cho Google và Facebook
- Mã hóa mật khẩu bằng BCrypt

### 6.2 Phân Quyền (Authorization)
- Role-based access control (RBAC)
- Roles: User, Admin
- Attribute-based authorization

### 6.3 Bảo Vệ Dữ Liệu
- HTTPS bắt buộc
- CSRF protection
- SQL Injection prevention (EF Core)
- XSS prevention (HTML encoding)
- Rate limiting

### 6.4 Quản Lý Phiên (Session Management)
- Session timeout: 30 phút
- Secure cookies
- HttpOnly flag

---

## 7. Hiệu Năng

### 7.1 Caching
- In-memory caching cho dữ liệu thường xuyên truy cập
- Output caching cho views tĩnh
- Database query optimization

### 7.2 Tối Ưu Hóa Cơ Sở Dữ Liệu
- Indexing trên các cột thường xuyên tìm kiếm
- Query optimization
- Connection pooling

### 7.3 Tối Ưu Hóa Frontend
- Minification CSS/JavaScript
- Image optimization
- Lazy loading

---

## 8. Deployment

### 8.1 Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "EnglishLearning.dll"]
```

### 8.2 Environments
- Development: Local machine
- Staging: Test server
- Production: Cloud server

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 27/03/2026  
**Trạng thái**: Draft

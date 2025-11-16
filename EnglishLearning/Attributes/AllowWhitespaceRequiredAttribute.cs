using EnglishLearning.Models;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearning.Attributes
{
    public class AllowWhitespaceRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Lấy instance của Quiz
            var quiz = validationContext.ObjectInstance as Quiz;
            if (quiz == null) return ValidationResult.Success;

            // Lấy LessonType từ Lesson liên quan
            var lessonType = quiz.Lesson?.LessonType;
            if (string.IsNullOrEmpty(lessonType)) return ValidationResult.Success; // Chưa có LessonType, bỏ qua

            // Nếu là TOEIC, cho phép null hoặc chuỗi trống/khoảng trắng
            if (lessonType == "TOEIC")
            {
                return ValidationResult.Success;
            }

            // Nếu là Normal, kiểm tra giá trị
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                return new ValidationResult(ErrorMessage ?? "Trường này là bắt buộc cho bài học Normal.");
            }

            return ValidationResult.Success;
        }
    }
}
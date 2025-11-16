using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EnglishLearning.Attributes;

namespace EnglishLearning.Models
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        public string? LessonId { get; set; }

        [AllowWhitespaceRequired]
        [StringLength(200)]
        public string? Question { get; set; } = string.Empty;

        [AllowWhitespaceRequired]
        public string? OptionA { get; set; } = string.Empty;

        [AllowWhitespaceRequired]
        public string? OptionB { get; set; } = string.Empty;

        [AllowWhitespaceRequired]
        public string? OptionC { get; set; } = string.Empty;

        [AllowWhitespaceRequired]
        public string? OptionD { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ImagePath { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public string? CorrectAnswer { get; set; } = string.Empty;

        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }
    }
}
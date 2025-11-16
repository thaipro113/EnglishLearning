using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Models
{
    public partial class Course
    {
        [Key]
        [StringLength(10)]
        [Required(ErrorMessage = "Mã khóa học là bắt buộc.")]
        public string CourseId { get; set; } = null!;

        [Required(ErrorMessage = "Tên khóa học là bắt buộc.")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Cấp độ là bắt buộc.")]
        public int? SubLevelId { get; set; } // Sử dụng SubLevelId làm khóa ngoại chính

        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("SubLevelId")]
        [InverseProperty("Courses")]
        public virtual SubLevel? SubLevel { get; set; } // Navigation property

        [InverseProperty("Course")]
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        // Loại bỏ LevelId và quan hệ với LearningLevel vì đã thay bằng SubLevel
    }
}
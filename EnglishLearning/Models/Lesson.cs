using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Models;

public partial class Lesson
{
    [Key]
    [StringLength(20)]
    [Required(ErrorMessage = "Mã bài học là bắt buộc.")]
    public string LessonId { get; set; } = null!;

    public string? CourseId { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    [StringLength(200)]
    public string? VideoUrl { get; set; }

    public int? OrderIndex { get; set; }
    public string? LessonType { get; set; }
    [StringLength(200)]
    public string? AudioPath { get; set; }
    [NotMapped]
    public IFormFile? AudioFile { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Lessons")]
    public virtual Course? Course { get; set; }

    [InverseProperty("Lesson")]
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    [InverseProperty("Lesson")]
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();


}

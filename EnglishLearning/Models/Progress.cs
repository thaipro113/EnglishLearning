using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Models;

public partial class Progress
{
    [Key]
    public int ProgressId { get; set; }

    public int UserId { get; set; }

    public string LessonId { get; set; }

    public bool IsCompleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CompletedAt { get; set; }
    public float? Score { get; set; }

    [ForeignKey("LessonId")]
    [InverseProperty("Progresses")]
    public virtual Lesson? Lesson { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Progresses")]
    public virtual User? User { get; set; }
}

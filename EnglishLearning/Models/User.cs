using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(10)]
    [Required]
    [Phone]
    public string? PhoneNumber { get; set; }


    [StringLength(50)]
    public string? Role { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    public string? Occupation { get; set; }
    public string? Level { get; set; }
    public string? Purpose { get; set; }
    public string? ImageUrl { get; set; } // Lưu đường dẫn hình ảnh

    // Thuộc tính tạm để xử lý upload file
    [NotMapped] // Không ánh xạ vào database
    public IFormFile? ImageFile { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    [InverseProperty("User")]
    public virtual ICollection<Paragraph> Paragraphs { get; set; } = new List<Paragraph>();

    [InverseProperty("User")]
    public virtual ICollection<TranslationHistory> TranslationHistories { get; set; } = new List<TranslationHistory>();
}

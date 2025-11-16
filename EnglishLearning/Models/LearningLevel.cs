using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Models;

public partial class LearningLevel
{
    [Key]
    public int LevelId { get; set; }

    [StringLength(100)]
    public string LevelName { get; set; } = null!;

    [InverseProperty("Level")]
    public virtual ICollection<SubLevel> SubLevels { get; set; } = new List<SubLevel>();

}

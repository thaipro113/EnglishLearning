using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearning.Models
{
    public class SubLevel
    {
        [Key]
        public int SubLevelId { get; set; }

        [Required]
        [StringLength(10)]
        public string SubLevelName { get; set; } = null!;

        public int LevelId { get; set; }

        [ForeignKey("LevelId")]
        public virtual LearningLevel Level { get; set; } = null!;
        [InverseProperty("SubLevel")]
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }

}

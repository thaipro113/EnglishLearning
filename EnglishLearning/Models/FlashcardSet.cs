using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearning.Models
{
    public class FlashcardSet
    {
        [Key]
        public int FlashcardSetId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }
        
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public virtual ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}

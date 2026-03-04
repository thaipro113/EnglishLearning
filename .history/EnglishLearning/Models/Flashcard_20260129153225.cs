using System.ComponentModel.DataAnnotations;

namespace EnglishLearning.Models
{
    public class Flashcard
    {
        [Key]
        public int FlashcardId { get; set; }
        
        public int FlashcardSetId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập nội dung mặt trước")]
        [Display(Name = "Mặt trước")]
        public string FrontText { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập nội dung mặt sau")]
        [Display(Name = "Mặt sau")]
        public string BackText { get; set; }
        
        public virtual FlashcardSet FlashcardSet { get; set; }
    }
}

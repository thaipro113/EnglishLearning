using Microsoft.AspNetCore.Mvc;

namespace EnglishLearning.Models
{
    public class Paragraph
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Level { get; set; }
        public string Topic { get; set; }
        public string ParagraphText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

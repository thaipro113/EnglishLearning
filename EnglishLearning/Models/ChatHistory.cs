namespace EnglishLearning.Models
{
    public class ChatHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // ID người dùng đăng nhập
        public string Role { get; set; } // "user" hoặc "assistant"
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public User? User { get; set; }  // Navigation property
    }

}

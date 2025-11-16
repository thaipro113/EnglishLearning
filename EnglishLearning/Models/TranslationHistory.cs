        namespace EnglishLearning.Models
        {
            public class TranslationHistory
            {
                public int Id { get; set; }
                public int UserId { get; set; }
                public User User { get; set; }
                public int ParagraphId { get; set; }
                public Paragraph Paragraph { get; set; }// New property to link to Paragraph
                public string Topic { get; set; }
                public string OriginalSentence { get; set; }
                public string UserTranslation { get; set; }
                public string StandardTranslation { get; set; }
                public double Accuracy { get; set; }
                public string ErrorSuggestion { get; set; }
                public DateTime Timestamp { get; set; }
            }
        }

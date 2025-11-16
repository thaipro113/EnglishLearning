namespace EnglishLearning.Models
{
    public class ParagraphData
    {
        public string Topic { get; set; }
        public string Level { get; set; }
        public string Paragraph { get; set; }
        public List<string> Sentences { get; set; }
        public List<string> StandardTranslations { get; set; }
    }
}

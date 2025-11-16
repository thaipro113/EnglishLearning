namespace EnglishLearning.Models
{
    public class GrammarCheckViewModel
    {
        public string InputText { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string CorrectedText { get; set; }
        public List<ErrorDetail> ErrorDetails { get; set; } = new List<ErrorDetail>();
    }

    public class ErrorDetail
    {
        public int OriginalOffset { get; set; }
        public int OriginalLength { get; set; }
        public int CorrectedOffset { get; set; }
        public int CorrectedLength { get; set; }
        public bool IsCorrected { get; set; }
        public string Suggestion { get; set; }
    }
}
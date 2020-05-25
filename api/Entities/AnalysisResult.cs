namespace API.Entities
{
    public class AnalysisResult
    {
        public enum Level
        {
            Suggestion,
            Warning,
            Error
        }

        public Level ResultLevel { get; set; }
        public string Message { get; set; }
    }
}
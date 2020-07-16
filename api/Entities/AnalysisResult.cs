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

        public enum Type
        {
            ExpiredLicense,
            MissingExpirationDate,
            NoLicensesFound,
            MismatchedIp,
            MismatchedMac,
            MismatchedIsvPort,
            AllLicensesInUse,
            NearlyAllLicensesInUse,
            MultipleRlmInstances
        }

        public Level ResultLevel { get; set; }
        public Type ResultType { get; set; }
        public string Message { get; set; }
    }
}
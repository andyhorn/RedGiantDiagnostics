namespace API.Contracts.Requests
{
    public class LogUpdateRequest : ILogUpdateRequest
    {
        public string Title { get; set; } = null;
        public string Comments { get; set; } = null;
    }
}
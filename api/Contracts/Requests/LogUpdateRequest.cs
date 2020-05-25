namespace API.Contracts.Requests
{
    public class LogUpdateRequest : ILogUpdateRequest
    {
        public string OwnerId { get; set; } = null;
        public string Title { get; set; } = null;
        public string Comments { get; set; } = null;
    }
}
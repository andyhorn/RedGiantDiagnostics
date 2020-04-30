namespace API.Contracts
{
    public class LogUpdateRequest
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
    }
}
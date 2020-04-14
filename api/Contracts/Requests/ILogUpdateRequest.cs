using API.Models;

namespace API.Contracts.Requests
{
    public interface ILogUpdateRequest
    {
        ILogFile Log { get; set; }
    }
}
using System.Collections.Generic;

namespace API.Models
{
    public interface IRlmStatistics : IServerStatistics
    {
        IEnumerable<IServerStatus> Servers { get; set; }
    }
}
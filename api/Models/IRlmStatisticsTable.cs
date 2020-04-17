using System.Collections.Generic;

namespace API.Models
{
    public interface IRlmStatisticsTable : IServerStatisticsTable
    {
        IEnumerable<IServerStatus> Servers { get; set; }
    }
}
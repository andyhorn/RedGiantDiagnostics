using System.Collections.Generic;

namespace API.Models
{
    public interface IRlmStatisticsTable : IStatisticsTable
    {
        IEnumerable<IServerStatus> Servers { get; set; }
    }
}
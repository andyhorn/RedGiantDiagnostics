using API.Entities;
using API.Models;

namespace API.Factories
{
    public interface ILogFactory
    {
        LogFile New();
        LogFile Parse(string data);
    }
}
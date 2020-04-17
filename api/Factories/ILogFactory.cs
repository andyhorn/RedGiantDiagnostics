using API.Models;

namespace API.Factories
{
    public interface ILogFactory
    {
        ILogFile New();
        ILogFile Parse(string data);
    }
}
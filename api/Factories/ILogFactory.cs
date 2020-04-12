using API.Models;

namespace API.Factories
{
    public interface ILogFactory
    {
        ILogFile GetNew();
        ILogFile Parse(string data);
    }
}
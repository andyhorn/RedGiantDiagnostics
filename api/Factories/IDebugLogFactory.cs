using API.Entities;

namespace API.Factories
{
    public interface IDebugLogFactory
    {
        DebugLog New { get; }
        DebugLog Parse(string[] data);
    }
}
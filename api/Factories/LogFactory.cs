using System;
using API.Entities;
using API.Models;

namespace API.Factories
{
    public class LogFactory : ILogFactory
    {
        public ILogFile GetNew() => new LogFile();

        public ILogFile Parse(string data)
        {
            throw new NotImplementedException();
        }
    }
}
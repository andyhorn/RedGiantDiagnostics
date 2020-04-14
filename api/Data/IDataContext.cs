//  File:           IDataContext.cs
//  Author:         Andy Horn
//  Description:    An interface that exposes an IMongoCollection<ILogFile> property.

using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public interface IDataContext
    {
        IMongoCollection<ILogFile> Logs { get; }
    }
}
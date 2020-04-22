//  File:           IDataContext.cs
//  Author:         Andy Horn
//  Description:    An interface that exposes an IMongoCollection<ILogFile> property.

using API.Entities;
using API.Models;
using MongoDB.Driver;

namespace API.Data
{
    public interface IDataContext
    {
        IMongoCollection<LogFile> Logs { get; }
    }
}
using NUnit.Framework;
using FakeItEasy;
using API.Data;
using MongoDB.Driver;
using API.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using API.Entities;

namespace api.test
{
    public class LogsRepositoryTest
    {
        private ILogsRepository _logsRepository; // System Under Test
        private IDataContext _context; // Mock
        private IMongoCollection<ILogFile> _logCollection; // Mock

        [SetUp]
        public void Setup()
        {
            // Set up a dummy data context
            _context = A.Fake<IDataContext>();

            // Set up a dummy mongo collection
            _logCollection = A.Fake<IMongoCollection<ILogFile>>();

            // Return the dummy mongo collection from the dummy context's Logs property
            A.CallTo(() => _context.Logs).Returns(_logCollection);

            // Instantiate a real repository
            _logsRepository = new LogsRepository(_context);
        }

        [Test]
        public async Task LogsRepository_GetAllLogs_CallsFindAsync()
        {
            // Arrange            
            // Act
            var list = await _logsRepository.GetAllLogsAsync();

            // Assert
            A.CallTo(_logCollection).Where(x => x.Method.Name == "FindAsync").MustHaveHappenedOnceExactly();
        }

        
    }
}
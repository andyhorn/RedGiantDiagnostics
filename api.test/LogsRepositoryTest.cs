//  File:               LogsRepositoryTest.cs
//  Author:             Andy Horn
//  Overview:           This class tests the LogsRepository class of the API project.
//                      Since the LogsRepository class essentially wraps the MongoDB
//                      driver, there is no need for thorough unit testing; However,
//                      this will test that the basic MongoClient methods are called
//                      as expected.


using NUnit.Framework;
using FakeItEasy;
using API.Data;
using MongoDB.Driver;
using API.Models;
using System.Threading.Tasks;
using Moq;

namespace api.test
{
    public class LogsRepositoryTest
    {
        private ILogsRepository _logsRepository; // System Under Test
        private IDataContext _context; // Mock
        private IMongoCollection<ILogFile> _logCollection; // Mock
        private Mock<IAsyncCursor<ILogFile>> _logCursor; // Mock

        [SetUp]
        public void Setup()
        {
            // Set up a dummy data context
            _context = A.Fake<IDataContext>();

            // Set up a dummy mongo collection
            _logCollection = A.Fake<IMongoCollection<ILogFile>>();

            // Return the dummy collection from the context's Logs property
            A.CallTo(() => _context.Logs).Returns(_logCollection);

            // Instantiate a real repository
            _logsRepository = new LogsRepository(_context);
        }

        [Test]
        public async Task LogsRepository_GetAllLogs_CallsMongoFindAsync()
        {
            // Arrange

            // Act
            var list = await _logsRepository.GetAllLogsAsync();

            // Assert
            A.CallTo(_logCollection)
                .Where(x => x.Method.Name == "FindAsync")
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsRepository_GetById_CallsMongoFindAsync()
        {
            // Arrange
            var dummyId = A.Dummy<string>();

            // Act
            var item = await _logsRepository.GetByIdAsync(dummyId);

            // Assert
            A.CallTo(_logCollection)
                .Where(x => x.Method.Name == "FindAsync")
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsRepository_RemoveAsync_CallsMongoFindOneAndDeleteAsync()
        {
            // Arrange
            var dummyId = A.Dummy<string>();

            // Act
            await _logsRepository.RemoveAsync(dummyId);

            // Assert
            A.CallTo(_logCollection)
                .Where(x => x.Method.Name == "FindOneAndDeleteAsync")
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsRepository_SaveAsync_CallsMongoInsertOneAsync()
        {
            // Arrange
            var dummyLog = A.Dummy<ILogFile>();

            // Act
            await _logsRepository.SaveAsync(dummyLog);

            // Assert
            A.CallTo(_logCollection)
                .Where(x => x.Method.Name == "InsertOneAsync")
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsRepository_UpdateAsync_CallsMongoFindOneAndReplaceAsync_And_FindAsync()
        {
            // Arrange
            var dummyLog = A.Dummy<ILogFile>();

            // Act
            await _logsRepository.UpdateAsync(dummyLog);

            // Assert
            A.CallTo(_logCollection)
                .Where(x => x.Method.Name == "FindOneAndReplaceAsync")
                .MustHaveHappenedOnceExactly();

            A.CallTo(_logCollection)
                .Where(x => x.Method.Name == "FindAsync")
                .MustHaveHappenedOnceExactly();
        }
    }
}
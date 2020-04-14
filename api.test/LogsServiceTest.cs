//  File:           LogsServiceTest.cs
//  Author:         Andy Horn
//  Description:    Tests the LogsService implementation.

using API.Data;
using NUnit.Framework;
using FakeItEasy;
using API.Models;
using System.Threading.Tasks;
using API.Services;
using System.Linq;

namespace api.test
{
    public class LogsServiceTest
    {
        private ILogsRepository _logsRepository; // Mock

        private ILogService _logService; // System Under Test

        [SetUp]
        public void Setup()
        {
            _logsRepository = A.Fake<ILogsRepository>();
            _logService = new LogService(_logsRepository);
        }

        [Test]
        public async Task LogsService_GetAllLogsAsync_ReturnsLogList()
        {
            // Arrange
            const int numLogs = 5;
            var fakeList = A.CollectionOfFake<ILogFile>(numLogs).ToList();
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).Returns(Task.FromResult(fakeList));

            // Act
            var list = await _logService.GetAllLogsAsync();

            // Assert
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(numLogs, list.ToList().Count);
        }

        
    }
}
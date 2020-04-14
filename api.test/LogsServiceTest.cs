using API.Data;
using NUnit.Framework;
using FakeItEasy;
using System.Collections.Generic;
using API.Models;
using API.Entities;
using System.Threading.Tasks;
using API.Services;

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
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).Returns(
                new List<ILogFile>
                {
                    new LogFile
                    {
                        Id = A.Dummy<string>()
                    },
                    new LogFile
                    {
                        Id = A.Dummy<string>()
                    },
                    new LogFile
                    {
                        Id = A.Dummy<string>()
                    }
                }
            );

            // Act
            var list = await _logService.GetAllLogsAsync();

            // Assert
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsNotEmpty(list);
        }

        
    }
}
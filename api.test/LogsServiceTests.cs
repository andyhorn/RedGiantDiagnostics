//  File:           LogsServiceTests.cs
//  Author:         Andy Horn
//  Description:    Tests the LogsService implementation.

using API.Data;
using NUnit.Framework;
using FakeItEasy;
using System.Threading.Tasks;
using API.Services;
using System.Linq;
using API.Factories;
using API.Entities;
using System;
using API.Contracts.Requests;
using API.Exceptions;
using API.Contracts.Requests.Admin;

namespace api.test
{

    public class LogsServiceTests
    {
        private ILogsRepository _logsRepository; // Mock
        private ILogFactory _logFactory; // Mock

        private ILogsService _logsService; // System Under Test

        [SetUp]
        public void Setup()
        {
            _logFactory = A.Fake<ILogFactory>();
            _logsRepository = A.Fake<ILogsRepository>();
            _logsService = new LogsService(_logsRepository, _logFactory);
        }

        [Test]
        public async Task LogsService_GetAllLogsAsync_ReturnsLogList()
        {
            // Arrange
            const int numLogs = 5;
            var fakeList = A.CollectionOfFake<LogFile>(numLogs).ToList();
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).Returns(Task.FromResult(fakeList));

            // Act
            var list = await _logsService.GetAllLogsAsync();

            // Assert
            Assert.AreEqual(numLogs, list.ToList().Count);
        }

        [Test]
        public async Task LogsService_GetAllLogsAsync_ReturnsEmptyList()
        {
            // Arrange
            var fakeList = A.CollectionOfFake<LogFile>(0).ToList();
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).Returns(Task.FromResult(fakeList));

            // Act
            var list = (await _logsService.GetAllLogsAsync()).ToList();

            // Assert
            Assert.IsEmpty(list);
        }

        [Test]
        public void LogsService_CreateAsync_NullLogFileObject_ThrowsException()
        {
            // Arrange
            LogFile log = null;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.CreateAsync(log));
        }

        [Test]
        public void LogsService_CreateAsync_AddError_ThrowsException()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsRepository.SaveAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _logsService.CreateAsync(log));
        }

        [Test]
        public async Task LogsService_GetByIdAsync_ReturnsValidLogFile()
        {
            // Arrange
            var fakeLog = A.Fake<LogFile>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(Task.FromResult(fakeLog));

            // Act
            var retrieved = await _logsService.GetByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(LogFile), retrieved);
        }

        [Test]
        public void LogsService_GetByIdAsync_NullIdThrowsException()
        {
            // Arrange
            const string nullId = null;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.GetByIdAsync(nullId));
        }

        [Test]
        public void LogsService_GetByIdAsync_WhitespaceId_ThrowsException()
        {
            // Arrange
            const string emptyId = "    ";

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.GetByIdAsync(emptyId));
        }

        [Test]
        public async Task LogsService_GetByIdAsync_ReturnsNullOnNoMatchingId()
        {
            // Arrange
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(Task.FromResult<LogFile>(null));

            // Act
            var result = await _logsService.GetByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void LogsService_LogExists_EmptyIdString_ThrowsException()
        {
            // Arrange
            var id = string.Empty;

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.LogExists(id));
        }

        [Test]
        public async Task LogsService_LogExists_NoMatch_ReturnsFalse()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns((LogFile)null);

            // Act
            var result = await _logsService.LogExists(id);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task LogsService_LogExists_MatchingLog_ReturnsTrue()
        {
            // Arrange
            var id = A.Dummy<string>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(log);

            // Act
            var result = await _logsService.LogExists(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task LogsService_GetForUser_ReturnsList()
        {
            // Arrange
            const int numTotalLogs = 10;        // 10 total logs
            const int numMatchingLogs = 5;      // 5 of which will belong to the 'owner'
            const string ownerId = "OwnerId";   // The 'owner' ID to use

            // Create the full list of logs
            var fakeList = A.CollectionOfFake<LogFile>(numTotalLogs).ToList();

            // Set the owner ID on five of the logs in the list
            for (var i = 0; i < numMatchingLogs; i++)
            {
                fakeList[i].OwnerId = ownerId;
            }

            // Make sure the repository uses this fake list
            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .Returns(Task.FromResult(fakeList));


            // Act
            // This should only return logs with a matching owner ID
            var list = await _logsService.GetForUserAsync(ownerId);

            // Assert
            Assert.AreEqual(numMatchingLogs, list.Count());
        }

        [Test]
        public async Task LogsService_GetForUser_ReturnsEmptyList()
        {
            // Arrange

            // Create a list of fake logs, give each one an ID
            var fakeList = A.CollectionOfFake<LogFile>(10).ToList();
            for (var i = 0; i < fakeList.Count; i++)
            {
                fakeList[i].OwnerId = i.ToString();
            }

            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .Returns(Task.FromResult(fakeList));

            // Act
            var result = await _logsService.GetForUserAsync(A.Dummy<string>());

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void LogsService_GetForUserAsync_NullUserId_ThrowsException()
        {
            // Arrange
            const string nullUserId = null;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.GetForUserAsync(nullUserId));
        }

        [Test]
        public void LogsService_GetForUserAsync_WhitespaceUserId_ThrowsException()
        {
            // Arrange
            const string emptyUserId = "    ";

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.GetForUserAsync(emptyUserId));
        }

        [Test]
        public async Task LogsService_DeleteAsync_RemovesItemFromList()
        {
            // Arrange
            const int initialCount = 10;
            var count = initialCount;
            var id = A.Dummy<string>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .Invokes(() => count--);

            // Act
            await _logsService.DeleteAsync(id);

            // Assert
            Assert.AreEqual(initialCount - 1, count);
        }

        [Test]
        public void LogsService_DeleteAsync_NullId_ThrowsException()
        {
            // Arrange
            const string nullId = null;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.DeleteAsync(nullId));
        }

        [Test]
        public void LogsService_DeleteAsync_EmptyId_ThrowsException()
        {
            // Arrange
            const string emptyId = "    ";

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.DeleteAsync(emptyId));
        }

        [Test]
        public void LogsService_DeleteAsync_NoMatch_ThrowsException()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns((LogFile)null);

            // Act and Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _logsService.DeleteAsync(id));
        }

        [Test]
        public void LogsService_UpdateAsync_EmptyIdString_ThrowsException()
        {
            // Arrange
            var id = string.Empty;
            var request = A.Dummy<LogUpdateRequest>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.UpdateAsync(id, request));
        }

        [Test]
        public void LogsService_UpdateAsync_NullLogUpdate_ThrowsException()
        {
            // Arrange
            var id = A.Dummy<string>();
            LogUpdateRequest request = null;

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _logsService.UpdateAsync(id, request));
        }

        [Test]
        public void LogsService_UpdateAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var id = A.Dummy<string>();
            var request = A.Dummy<LogUpdateRequest>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns((LogFile)null);

            // Act and Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _logsService.UpdateAsync(id, request));
        }

        [Test]
        public async Task LogsService_UpdateAsync_CallsRepositoryUpdate()
        {
            // Arrange
            var id = A.Dummy<string>();
            var request = A.Dummy<LogUpdateRequest>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .Returns(log);

            // Act
            var result = await _logsService.UpdateAsync(id, request);

            // Assert
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsService_UpdateAsync_ReturnsSameLogFile()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            var logFile = A.Fake<LogFile>();
            const string id = "fakeId";
            logFile.Id = id;
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(logFile);
            A.CallTo(() => _logsRepository.UpdateAsync(logFile))
                .Returns(logFile);

            // Act
            var result = await _logsService.UpdateAsync(id, request);

            // Assert
            Assert.AreSame(logFile, result);
        }

        [Test]
        public async Task LogsService_UpdateAsync_HandlesAdminUpdateRequest()
        {
            // Arrange
            var id = A.Dummy<string>();
            var request = A.Dummy<AdminLogUpdateRequest>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .Returns(log);

            // Act
            var result = await _logsService.UpdateAsync(id, request);

            // Assert
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void LogsService_Parse_ParsesData()
        {
            // Arrange
            A.CallTo(() => _logFactory.Parse(A<string>.Ignored))
                .Returns(A.Fake<LogFile>());

            // Act
            var newLog = _logsService.Parse(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(LogFile), newLog);
            A.CallTo(() => _logFactory.Parse(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void LogsService_Parse_NullString_ThrowsException()
        {
            // Arrange
            const string nullString = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _logsService.Parse(nullString));
        }
    }
}
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
using API.Factories;
using API.Entities;
using System;

namespace api.test
{

    public class LogsServiceTest
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
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsNotEmpty(list);
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
            A.CallTo(() => _logsRepository.GetAllLogsAsync()).MustHaveHappenedOnceExactly();
            Assert.IsEmpty(list);
        }

        [Test]
        public async Task LogsService_CreateAsync_AddsToList()
        {
            // Arrange
            const int numOriginalLogs = 3;
            var fakeLog = A.Fake<LogFile>();
            var list = A.CollectionOfFake<LogFile>(numOriginalLogs);
            A.CallTo(() => _logsRepository.SaveAsync(A<LogFile>.Ignored))
                .Invokes((callObject) => {
                    list.Add(callObject.FakedObject as LogFile);
                });

            // Act
            var result = await _logsService.CreateAsync(fakeLog);

            // Assert
            Assert.IsNotNull(result);
            A.CallTo(() => _logsRepository.SaveAsync(A<LogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
            Assert.AreEqual(numOriginalLogs + 1, list.Count);
        }

        [Test]
        public async Task LogsService_CreateAsync_HandlesNullObject()
        {
            // Arrange
            LogFile nullObject = null;

            // Act
            var result = await _logsService.CreateAsync(nullObject);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.SaveAsync(A<LogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_GetByIdAsync_ReturnsLogWithValidId()
        {
            // Arrange
            var fakeLog = A.Fake<LogFile>();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .Returns(Task.FromResult(fakeLog));

            // Act
            var retrieved = await _logsService.GetByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsNotNull(retrieved);
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsService_GetByIdAsync_HandlesNullId()
        {
            // Arrange
            const string nullId = null;

            // Act
            var result = await _logsService.GetByIdAsync(nullId);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_GetByIdAsync_HandlesWhitespaceId()
        {
            // Arrange
            const string emptyId = "    ";

            // Act
            var result = await _logsService.GetByIdAsync(emptyId);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
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
            Assert.IsNotEmpty(list);
            Assert.AreEqual(numMatchingLogs, list.Count());
            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .MustHaveHappenedOnceExactly();
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
            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsService_GetForUserAsync_HandlesNullUserId()
        {
            // Arrange
            const string nullUserId = null;

            // Act
            var result = await _logsService.GetForUserAsync(nullUserId);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_GetForUserAsync_HandlesWhitespaceUserId()
        {
            // Arrange
            const string emptyUserId = "    ";

            // Act
            var result = await _logsService.GetForUserAsync(emptyUserId);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_DeleteAsync_RemovesItemFromList()
        {
            // Arrange
            const int originalCount = 10;
            const string idToRemove = "removeMe";
            var fakeList = A.CollectionOfFake<LogFile>(originalCount).ToList();
            fakeList[5].Id = idToRemove;
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .Invokes(call => {
                    var toRemove = fakeList.First(x => x.Id == (string)call.Arguments[0]);
                    fakeList.Remove(toRemove);
                });

            // Act
            await _logsService.DeleteAsync(idToRemove);

            // Assert
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            Assert.AreEqual(originalCount - 1, fakeList.Count);
        }

        [Test]
        public async Task LogsService_DeleteAsync_HandlesNullId()
        {
            // Arrange
            const string nullId = null;

            // Act
            await _logsService.DeleteAsync(nullId);

            // Assert
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_DeleteAsync_HandlesEmptyId()
        {
            // Arrange
            const string emptyId = "    ";

            // Act
            await _logsService.DeleteAsync(emptyId);

            // Assert
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_DeleteAsync_HandlesNonmatchingId()
        {
            // Arrange
            const string invalidId = "noMatchForMe";
            const int numLogs = 10;
            bool removed = false;
            var fakeList = A.CollectionOfFake<LogFile>(numLogs).ToList();

            // Remove any items with a matching ID; There shouldn't be any
            // with a matching ID, so none should be removed.
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .Invokes(call => {
                    var id = (string)call.Arguments[0];
                    var item = fakeList.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        fakeList.Remove(item);
                        removed = true;
                    }
                });
            
            // Act
            await _logsService.DeleteAsync(invalidId);

            // Assert
            A.CallTo(() => _logsRepository.RemoveAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            Assert.AreEqual(numLogs, fakeList.Count);
            Assert.IsFalse(removed);
        }

        [Test]
        public async Task LogsService_UpdateAsync_CallsRepositoryUpdate()
        {
            // Arrange
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .Returns(Task.FromResult(A.Fake<LogFile>()));

            // Act
            var result = await _logsService.UpdateAsync(A.Fake<LogFile>());

            // Assert
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsService_UpdateAsync_ReturnsSameLogFile()
        {
            // Arrange
            var logFile = A.Fake<LogFile>();
            const string id = "fakeId";
            logFile.Id = id;
            A.CallTo(() => _logsRepository.UpdateAsync(logFile))
                .Returns(logFile);

            // Act
            var result = await _logsService.UpdateAsync(logFile);

            // Assert
            Assert.AreSame(logFile, result);
        }

        [Test]
        public async Task LogsService_UpdateAsync_HandlesNullObject()
        {
            // Arrange 
            LogFile nullObject = null;
            
            // Act
            var result = await _logsService.UpdateAsync(nullObject);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.UpdateAsync(A<LogFile>.Ignored))
                .MustNotHaveHappened();
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
        public void LogsService_Parse_RejectsNullString()
        {
            // Arrange
            const string nullString = null;

            // Act
            var result = _logsService.Parse(nullString);

            // Assert
            Assert.IsNull(result);

            A.CallTo(() => _logFactory.Parse(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void LogsService_Parse_RejectsEmptyString()
        {
            // Arrange
            const string emptyString = "   ";

            // Act
            var result = _logsService.Parse(emptyString);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logFactory.Parse(A<string>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
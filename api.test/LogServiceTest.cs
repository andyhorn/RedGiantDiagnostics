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
using System.IO;
using API.Factories;

namespace api.test
{
    public class LogsServiceTest
    {
        private ILogsRepository _logsRepository; // Mock

        private ILogsService _logsService; // System Under Test

        [SetUp]
        public void Setup()
        {
            _logsRepository = A.Fake<ILogsRepository>();
            _logsService = new LogsService(_logsRepository);
        }

        [Test]
        public async Task LogsService_GetAllLogsAsync_ReturnsLogList()
        {
            // Arrange
            const int numLogs = 5;
            var fakeList = A.CollectionOfFake<ILogFile>(numLogs).ToList();
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
            var fakeList = A.CollectionOfFake<ILogFile>(0).ToList();
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
            var fakeLog = A.Fake<ILogFile>();
            var list = A.CollectionOfFake<ILogFile>(numOriginalLogs);
            A.CallTo(() => _logsRepository.SaveAsync(A<ILogFile>.Ignored))
                .Invokes((callObject) => {
                    list.Add(callObject.FakedObject as ILogFile);
                });

            // Act
            var result = await _logsService.CreateAsync(fakeLog);

            // Assert
            Assert.IsNotNull(result);
            A.CallTo(() => _logsRepository.SaveAsync(A<ILogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
            Assert.AreEqual(numOriginalLogs + 1, list.Count);
        }

        [Test]
        public async Task LogsService_CreateAsync_HandlesNullObject()
        {
            // Arrange
            ILogFile nullObject = null;

            // Act
            var result = await _logsService.CreateAsync(nullObject);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.SaveAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsService_GetByIdAsync_ReturnsLogWithValidId()
        {
            // Arrange
            var fakeLog = A.Fake<ILogFile>();
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
                .Returns(Task.FromResult<ILogFile>(null));

            // Act
            var result = await _logsService.GetByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsService_GetForUser_ReturnsList()
        {
            // Arrange
            const int numTotalLogs = 10;        // 10 total logs
            const int numMatchingLogs = 5;      // 5 of which will belong to the 'owner'
            const string ownerId = "OwnerId";   // The 'owner' ID to use

                // Create the full list of logs
            var fakeList = A.CollectionOfFake<ILogFile>(numTotalLogs).ToList();

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

            // None of the logs will have a matching OwnerId
            var fakeList = A.CollectionOfFake<ILogFile>(10).ToList();
            A.CallTo(() => _logsRepository.GetAllLogsAsync())
                .Returns(Task.FromResult(fakeList));

            // Act
            var result = await _logsService.GetForUserAsync("ownerId");

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
            var fakeList = A.CollectionOfFake<ILogFile>(originalCount).ToList();
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
            var fakeList = A.CollectionOfFake<ILogFile>(numLogs).ToList();

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
            A.CallTo(() => _logsRepository.UpdateAsync(A<ILogFile>.Ignored))
                .Returns(Task.FromResult(A.Fake<ILogFile>()));

            // Act
            var result = await _logsService.UpdateAsync(A.Fake<ILogFile>());

            // Assert
            A.CallTo(() => _logsRepository.UpdateAsync(A<ILogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsService_UpdateAsync_ReturnsSameLogFile()
        {
            // Arrange
            var logFile = A.Fake<ILogFile>();
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
            ILogFile nullObject = null;
            
            // Act
            var result = await _logsService.UpdateAsync(nullObject);

            // Assert
            Assert.IsNull(result);
            A.CallTo(() => _logsRepository.UpdateAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void LogsService_Parse_ParsesData()
        {
            // Arrange
            var factory = A.Fake<ILogFactory>();

            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Invokes(call => factory.Parse((string)call.Arguments[0]));

            A.CallTo(() => factory.Parse(A<string>.Ignored))
                .Returns(A.Fake<ILogFile>());

            // Act
            var newLog = _logsService.Parse(A.Fake<string>());

            // Assert
            Assert.IsInstanceOf(typeof(ILogFile), newLog);
        }
    }
}
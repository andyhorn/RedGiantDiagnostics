using NUnit.Framework;
using FakeItEasy;
using API.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Services;
using API.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using API.Entities;
using API.Contracts;

namespace api.test
{
    public class LogsControllerTests
    {
        private class DummyStringFactory : DummyFactory<string>
        {
            protected override string Create() => "DummyString";
        }

        private class DummyLogFactory : DummyFactory<LogFile>
        {
            protected override LogFile Create()
            {
                return new LogFile()
                {
                    Id = "1",
                    Date = DateTime.Now,
                    RlmVersion = "100",
                    Hostname = "DummyLog"
                };
            }
        }
        private LogsController _logsController;
        private ILogsService _logsService;
        private IFileService _fileService;

        [SetUp]
        public void Setup()
        {
            _logsService = A.Fake<ILogsService>();
            _fileService = A.Fake<IFileService>();
            _logsController = new LogsController(_logsService, _fileService);
        }

        [Test]
        public async Task LogsController_Get()
        {
            // Arrange
            const int numLogs = 5;
            var fakeCollection = A.CollectionOfDummy<LogFile>(numLogs).ToList();
            A.CallTo(() => _logsService.GetAllLogsAsync())
                .Returns(fakeCollection);

            // Act
            var result = await _logsController.Get();

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var data = okResult.Value as IEnumerable<LogSummaryResponse>;

            Assert.AreEqual(numLogs, data.Count());
        }

        [Test]
        public async Task LogsController_GetById_ValidIdReturnsItem()
        {
            // Arrange
                // Simulate a valid id being passed and returning a log object
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(Task.FromResult(A.Fake<LogFile>()));

            // Act
            var result = await _logsController.GetById(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var data = result as OkObjectResult;
            var content = (LogFile)data.Value;

            // var log = JsonConvert.DeserializeObject<LogFile>(content);
            Assert.IsInstanceOf(typeof(LogFile), content);

            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_GetById_HandlesNullId()
        {
            // Arrange
            const string nullId = null;

            // Act
            var result = await _logsController.GetById(nullId);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_GetById_HandlesInvalidId()
        {
            // Arrange
            // Simulate not finding a match for the ID
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns<LogFile>(null);

            // Act
            var result = await _logsController.GetById(A.Dummy<string>());

            // Assert
            // Sending an invalid (non-matching) ID should result in a 404 Not Found
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_GetForUser_ReturnsOkObjectResult()
        {
            // Arrange
            var logs = A.CollectionOfDummy<LogFile>(5).ToList();
            var userId = A.Dummy<string>();

            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .Returns(logs);

            // Act
            var result = await _logsController.GetForUser(userId);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task LogsController_GetForUser_ReturnsListOfLogSummary()
        {
            // Arrange
            const int numLogs = 5;
            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .Returns(A.CollectionOfDummy<LogFile>(numLogs));

            // Act
            var result = await _logsController.GetForUser(A.Dummy<string>());

            // Assert
            var okResult = result as OkObjectResult;
            
            Assert.IsInstanceOf(typeof(IEnumerable<LogSummaryResponse>), okResult.Value);
        }

        [Test]
        public async Task LogsController_GetForUser_HandlesNoMatchingLogs()
        {
            // Arrange
            var emptyList = A.CollectionOfFake<LogFile>(0);
            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .Returns(emptyList);

            // Act
            var result = await _logsController.GetForUser(A.Dummy<string>());
            var okResult = result as OkObjectResult;
            var data = okResult.Value as IEnumerable<LogSummaryResponse>;

            // Assert
            Assert.IsEmpty(data);
        }

        [Test]
        public async Task LogsController_GetForUser_HandlesNullUserId()
        {
            // Arrange

            // Act
            var result = await _logsController.GetForUser(null);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Update_ValidUpdateRequest()
        {
            // Arrange
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .Returns(A.Fake<LogFile>());

            // Act
            var result = await _logsController.Update(A.Dummy<string>(), A.Fake<LogFile>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var content = okResult.Value as LogFile;

            Assert.IsInstanceOf(typeof(LogFile), content);

            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_Update_HandlesNullId()
        {
            // Arrange
            const string nullId = null;

            // Act
            var result = await _logsController.Update(nullId, A.Fake<LogFile>());

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);

            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Update_HandlesNullLogUpdateRequestObject()
        {
            // Arrange
            const string id = "validIdString";
            LogFile nullUpdateRequest = null;

            // Act
            var result = await _logsController.Update(id, nullUpdateRequest);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Delete_ValidDeleteRequest()
        {
            // Arrange
            const int numLogs = 10;
            var fakeCollection = A.CollectionOfFake<LogFile>(numLogs).ToList();
            const string deleteId = "deleteMe";
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .Invokes(call => {
                    var id = (string)call.Arguments[0];

                    var item = fakeCollection.FirstOrDefault(x => x.Id == id);
                    if (item != null)
                    {
                        fakeCollection.Remove(item);
                    }
                });

            // Act
            var result = await _logsController.Delete(deleteId);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);

            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_Delete_NoMatchingId_ReturnsNotFound()
        {
            // Sending a delete request for a non-existent ID should return
            // a 404 Not Found error.

            // Arrange
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns<LogFile>(null);

            // Act
            var result = await _logsController.Delete(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_Delete_HandlesNullId()
        {
            // Arrange
            const string nullId = null;

            // Act
            var result = await _logsController.Delete(nullId);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);

            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Delete_HandlesEmptyId()
        {
            // Arrange
            const string emptyId = "    ";

            // Act
            var result = await _logsController.Delete(emptyId);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);

            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Save_ValidLogObjectSaves()
        {
            // Arrange
            int numLogs = 0;
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .Invokes(() => numLogs++)
                .Returns(A.Dummy<LogFile>());

            // Act
            var result = await _logsController.Save(A.Fake<LogFile>());

            // Assert
            Assert.IsInstanceOf(typeof(CreatedResult), result);

            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.AreEqual(1, numLogs);
        }

        [Test]
        public async Task LogsController_Save_HandlesNullLogObject()
        {
            // Arrange
            LogFile nullLog = null;

            // Act
            var result = await _logsController.Save(nullLog);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);

            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Save_ExistingObjectReturnsConflict()
        {
            // Arrange 
            const int numLogs = 5;
            const string logId = "iExist";
            var list = A.CollectionOfFake<LogFile>(5);
            list[0].Id = logId;
            var newLog = A.Fake<LogFile>();
            newLog.Id = logId;
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .Invokes(call => {
                    var log = (LogFile)call.Arguments[0];

                    if (log != null)
                    {
                        list.Add(log);
                    }
                });

            // Act
            var result = await _logsController.Save(newLog);

            // Assert
            Assert.IsInstanceOf(typeof(ConflictResult), result);

            Assert.AreEqual(numLogs, list.Count);

            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Upload_ValidUpload()
        {
            // Arrange
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .Returns(A.Dummy<string>());
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Returns(A.Dummy<LogFile>());
            var fakeFile = A.Fake<IFormFile>();

            // Act
            var result = await _logsController.Upload(fakeFile);

            // Assert
            Assert.IsInstanceOf(typeof(ObjectResult), result);

            var okResult = result as ObjectResult;
            var data = okResult.Value as string;
            LogFile json = JsonConvert.DeserializeObject<LogFile>(data);


            Assert.IsInstanceOf(typeof(LogFile), json);
            Assert.AreEqual("DummyLog", json.Hostname);
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_Upload_HandlesNullObject()
        {
            // Arrange
            IFormFile nullFile = null;

            // Act
            var result = await _logsController.Upload(nullFile);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Upload_HandlesEmptyFile()
        {
            // Arrange
            var fakeFile = A.Dummy<IFormFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .Throws(() => throw new ArgumentException());

            // Act
            var result = await _logsController.Upload(fakeFile);

            // Assert
            Assert.IsInstanceOf(typeof(ObjectResult), result);
            Assert.AreEqual(500, (result as ObjectResult).StatusCode);
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
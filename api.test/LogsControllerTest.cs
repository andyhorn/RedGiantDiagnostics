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
using API.Contracts.Requests;
using System.Linq;

namespace api.test
{
    public class LogsControllerTests
    {
        private class DummyStringFactory : DummyFactory<string>
        {
            protected override string Create() => "DummyString";
        }
        private LogsController _logsController;
        private ILogsService _logsService;

        [SetUp]
        public void Setup()
        {
            _logsService = A.Fake<ILogsService>();
            _logsController = new LogsController(_logsService);
        }

        [Test]
        public async Task LogsController_Get()
        {
            // Arrange
            const int numLogs = 5;
            var fakeCollection = A.CollectionOfFake<ILogFile>(numLogs).ToList();
            A.CallTo(() => _logsService.GetAllLogsAsync())
                .Returns(fakeCollection);

            // Act
            var result = await _logsController.Get();

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var data = okResult.Value as List<ILogFile>;

            Assert.AreEqual(numLogs, data.Count);
        }

        [Test]
        public async Task LogsController_GetById_ValidIdReturnsItem()
        {
            // Arrange
                // Simulate a valid id being passed and returning a log object
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(Task.FromResult(A.Fake<ILogFile>()));

            // Act
            var result = await _logsController.GetById(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var data = result as OkObjectResult;
            var content = (ILogFile)data.Value;

            // var log = JsonConvert.DeserializeObject<ILogFile>(content);
            Assert.IsInstanceOf(typeof(ILogFile), content);

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
                .Returns<ILogFile>(null);

            // Act
            var result = await _logsController.GetById(A.Dummy<string>());

            // Assert
            // Sending an invalid (non-matching) ID should result in a 404 Not Found
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_GetForUser_ReturnsList()
        {
            // Arrange
            const int numLogs = 5;
            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .Returns(A.CollectionOfDummy<ILogFile>(numLogs));

            // Act
            var result = await _logsController.GetForUser(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var data = okResult.Value as List<ILogFile>;

            Assert.IsInstanceOf(typeof(List<ILogFile>), data);
            Assert.AreEqual(numLogs, data.Count);

            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_GetForUser_HandlesNoMatchingLogs()
        {
            // Arrange
            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .Returns(new List<ILogFile>());

            // Act
            var result = await _logsController.GetForUser(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var data = okResult.Value as List<ILogFile>;

            Assert.IsInstanceOf(typeof(List<ILogFile>), data);
            Assert.IsEmpty(data);

            A.CallTo(() => _logsService.GetForUserAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
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
            A.CallTo(() => _logsService.UpdateAsync(A<ILogFile>.Ignored))
                .Returns(A.Fake<ILogFile>());

            // Act
            var result = await _logsController.Update(A.Dummy<string>(), A.Fake<ILogUpdateRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var content = okResult.Value as ILogFile;

            Assert.IsInstanceOf(typeof(ILogFile), content);

            A.CallTo(() => _logsService.UpdateAsync(A<ILogFile>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_Update_HandlesNullId()
        {
            // Arrange
            const string nullId = null;

            // Act
            var result = await _logsController.Update(nullId, A.Fake<ILogUpdateRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);

            A.CallTo(() => _logsService.UpdateAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Update_HandlesNullLogUpdateRequestObject()
        {
            // Arrange
            const string id = "validIdString";
            ILogUpdateRequest nullUpdateRequest = null;

            // Act
            var result = await _logsController.Update(id, nullUpdateRequest);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
            A.CallTo(() => _logsService.UpdateAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Delete_ValidDeleteRequest()
        {
            // Arrange
            const int numLogs = 10;
            var fakeCollection = A.CollectionOfFake<ILogFile>(numLogs).ToList();
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
                .Returns<ILogFile>(null);

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
            var list = new List<ILogFile>();
            A.CallTo(() => _logsService.CreateAsync(A<ILogFile>.Ignored))
                .Invokes(call => {
                    var log = (ILogFile)call.Arguments[0];

                    if (log != null)
                    {
                        list.Add(log);
                    }
                });
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns<ILogFile>(null);

            // Act
            var result = await _logsController.Save(A.Fake<ILogFile>());

            // Assert
            Assert.IsInstanceOf(typeof(CreatedResult), result);

            A.CallTo(() => _logsService.CreateAsync(A<ILogFile>.Ignored))
                .MustHaveHappenedOnceExactly();

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public async Task LogsController_Save_HandlesNullLogObject()
        {
            // Arrange
            ILogFile nullLog = null;

            // Act
            var result = await _logsController.Save(nullLog);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);

            A.CallTo(() => _logsService.CreateAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Save_ExistingObjectReturnsConflict()
        {
            // Arrange 
            const int numLogs = 5;
            const string logId = "iExist";
            var list = A.CollectionOfFake<ILogFile>(5);
            list[0].Id = logId;
            var newLog = A.Fake<ILogFile>();
            newLog.Id = logId;
            A.CallTo(() => _logsService.CreateAsync(A<ILogFile>.Ignored))
                .Invokes(call => {
                    var log = (ILogFile)call.Arguments[0];

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
            A.CallTo(() => _logsService.CreateAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Upload_ValidUpload()
        {
            // Arrange
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Returns(A.Fake<ILogFile>());

            // Act
            var result = await _logsController.Upload(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var okResult = result as OkObjectResult;
            var data = okResult.Value as ILogFile;

            Assert.IsInstanceOf(typeof(ILogFile), data);
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LogsController_Upload_HandlesNullObject()
        {
            // Arrange
            const string nullObject = null;

            // Act
            var result = await _logsController.Upload(nullObject);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task LogsController_Upload_HandlesEmptyString()
        {
            // Arrange
            const string emptyString = "   ";

            // Act
            var result = await _logsController.Upload(emptyString);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
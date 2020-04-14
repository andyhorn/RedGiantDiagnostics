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

namespace api.test
{
    public class LogsControllerTests
    {
        private LogsController _logsController;
        private ILogsService _logsService;

        [SetUp]
        public void Setup()
        {
            _logsService = A.Fake<ILogsService>();
            _logsController = new LogsController(_logsService);
        }

        [Test]
        public void LogsController_Get()
        {
            // Get the test endpiont
            var result = _logsController.Get();

            // Ensure it returns an Ok
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            
            // Get the content from the result
            var data = result as OkObjectResult;
            var content = (string)data.Value;

            // Ensure it contains the "Hello world!" string
            Assert.AreEqual("Hello world!", content);
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
            var content = (string)data.Value;

            var log = JsonConvert.DeserializeObject<ILogFile>(content);
            Assert.IsInstanceOf(typeof(ILogFile), log);

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
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
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
            var result = await _logsController.GetById(A.Fake<string>());

            // Assert
            // Sending an invalid (non-matching) ID should result in a 404 Not Found
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
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
            var result = await _logsController.GetForUser(A.Fake<string>());

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
            var result = await _logsController.GetForUser(A.Fake<string>());

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
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
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
            var result = await _logsController.Update(A.Fake<string>(), A.Fake<ILogUpdateRequest>());

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
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);

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
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
            A.CallTo(() => _logsService.UpdateAsync(A<ILogFile>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
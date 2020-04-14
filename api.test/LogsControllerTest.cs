using NUnit.Framework;
using FakeItEasy;
using API.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Services;
using API.Models;
using Newtonsoft.Json;

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
    }
}
using System;
using System.Threading.Tasks;
using API.Contracts;
using API.Controllers.V2;
using API.Entities;
using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace api.test
{
    public class LogsControllerV2Tests
    {
        private ILogsService _logsService;
        private IFileService _fileService;
        private LogsControllerV2 _controller;

        [SetUp]
        public void Setup()
        {
            _logsService = A.Fake<ILogsService>();
            _fileService = A.Fake<IFileService>();
            _controller = new LogsControllerV2(_logsService, _fileService);
        }

        [Test]
        public async Task LogsControllerV2_GetById_EmptyIdString_ReturnsBadRequest()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_GetById_EmptyIdString_BadRequestContainsErrorMessage()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var data = (result as BadRequestObjectResult).Value as string;
            Assert.IsNotEmpty(data);
        }

        [Test]
        public async Task LogsControllerV2_GetById_NoMatchFound_ReturnsNotFound()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task LogsControllerV2_GetById_MatchFound_ReturnsOk()
        {
            // Arrange
            var id = A.Dummy<string>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            
            // Act
            var result = await _controller.GetById(id);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_GetById_MatchFound_OkResultContainsLogData()
        {
            // Arrange
            var id = A.Dummy<string>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            
            // Act
            var result = await _controller.GetById(id);

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsInstanceOf(typeof(LogFile), data);
        }

        [Test]
        public async Task LogsControllerV2_Upload_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Upload_FileParsingError_ReturnsStatusCode()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Upload_FileParsingError_Returns500StatusCode()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task LogsControllerV2_Upload_LogParsingError_ReturnsStatusCode()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .Returns(A.Dummy<string>());
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Throws(new Exception());
            
            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Upload_LogParsingError_Returns500StatusCode()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .Returns(A.Dummy<string>());
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Throws(new Exception());
            
            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task LogsControllerV2_Upload_SuccessfulParse_ReturnsOk()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .Returns(A.Dummy<string>());
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Returns(log);

            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Upload_SuccessfulParse_OkResultContainsLogFile()
        {
            // Arrange
            var formFile = A.Fake<IFormFile>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _fileService.ReadFormFileAsync(A<IFormFile>.Ignored))
                .Returns(A.Dummy<string>());
            A.CallTo(() => _logsService.Parse(A<string>.Ignored))
                .Returns(log);

            // Act
            var result = await _controller.Upload(formFile);

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsInstanceOf(typeof(LogFile), data);
        }

        [Test]
        public async Task LogsControllerV2_Save_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Save(log);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Save_InvalidModelState_BadRequestContainsErrorMessages()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Save(log);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsInstanceOf(typeof(SerializableError), data);
        }

        [Test]
        public async Task LogsControllerV2_Save_HasIdAndConflictsWithExistingLog_ReturnsConflict()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.Save(log);

            // Assert
            Assert.IsInstanceOf(typeof(ConflictObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Save_HadIdAndConflictsWithExistingLog_ConflictContainsErrorMessage()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.Save(log);

            // Assert
            var data = (result as ConflictObjectResult).Value as string;
            Assert.IsNotEmpty(data);
        }

        [Test]
        public async Task LogsControllerV2_Save_SaveError_ReturnsStatusCode()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = null;
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Save(log);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Save_SaveError_Returns500StatusCode()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = null;
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Save(log);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task LogsControllerV2_Save_Success_ReturnsCreated()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = null;
            var savedLog = A.Dummy<LogFile>();
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .Returns(savedLog);

            // Act
            var result = await _controller.Save(log);

            // Assert
            Assert.IsInstanceOf(typeof(CreatedResult), result);
        }

        [Test]
        public async Task LogsControllerV2_Save_Success_CreatedResultContainsLogData()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = null;
            var savedLog = A.Dummy<LogFile>();
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .Returns(savedLog);

            // Act
            var result = await _controller.Save(log);

            // Assert
            var data = (result as CreatedResult).Value;
            Assert.IsInstanceOf(typeof(LogFile), data);
        }

        [Test]
        public async Task LogsControllerV2_Save_Success_CreatedResultContainsLogId()
        {
            // Arrange
            var log = A.Dummy<LogFile>();
            log.Id = null;
            var savedLog = A.Dummy<LogFile>();
            A.CallTo(() => _logsService.CreateAsync(A<LogFile>.Ignored))
                .Returns(savedLog);

            // Act
            var result = await _controller.Save(log);

            // Assert
            var data = (result as CreatedResult).Location;
            Assert.IsNotEmpty(data);
        }

        [Test]
        public async Task LogsControllerV2_UpdateLog_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Fake<LogUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_UpdateLog_InvalidModelState_BadRequestContainsErrorMessages()
        {
            // Arrange
            var request = A.Fake<LogUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsInstanceOf(typeof(SerializableError), data);
        }

        [Test]
        public async Task LogsControllerV2_UpdateLog_NoMatchingLog_ReturnsNotFound()
        {
            // Arrange
            var request = A.Fake<LogUpdateRequest>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task LogsControllerV2_UpdateLog_UpdateError_ReturnsStatusCode()
        {
            // Arrange
            var request = A.Fake<LogUpdateRequest>();
            var log = A.Fake<LogFile>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task LogsControllerV2_UpdateLog_UpdateError_Returns500StatusCode()
        {
            // Arrange
            var request = A.Fake<LogUpdateRequest>();
            var log = A.Fake<LogFile>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task LogsControllerV2_UpdateLog_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            var request = A.Fake<LogUpdateRequest>();
            var log = A.Fake<LogFile>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.GetByIdAsync(A<string>.Ignored))
                .Returns(log);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .Returns(log);

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task LogsControllerV2_DeleteLog_EmptyIdString_ReturnsBadRequest()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task LogsControllerV2_DeleteLog_EmptyIdString_BadRequestContainsErrorMessage()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            var data = (result as BadRequestObjectResult).Value as string;
            Assert.IsNotEmpty(data);
        }

        [Test]
        public async Task LogsControllerV2_DeleteLog_NoMatchingId_ReturnsNotFound()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task LogsControllerV2_DeleteLog_DeletionError_ReturnsStatusCode()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task LogsControllerV2_DeleteLog_DeletionError_Returns500StatusCode()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task LogsControllerV2_DeleteLog_DeletionSuccess_ReturnsNoContent()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            
            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }
    }
}
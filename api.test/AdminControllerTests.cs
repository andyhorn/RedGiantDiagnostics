using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts;
using API.Controllers.V2;
using API.Entities;
using API.Exceptions;
using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace api.test
{
    public class AdminControllerTests
    {
        private IIdentityService _identityService;
        private ILogsService _logsService;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _identityService = A.Fake<IIdentityService>();
            _logsService = A.Fake<ILogsService>();
            _controller = new AdminController(_identityService, _logsService);
        }

        #region AdminController.Users Tests

        [Test]
        public async Task AdminController_RegisterUser_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();

            _controller.ModelState.AddModelError(A.Dummy<string>(), "TestError");

            // Act
            var result = await _controller.RegisterUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_RegisterUser_InvalidModelState_ReturnsListOfErrorStrings()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();
            _controller.ModelState.AddModelError(A.Dummy<string>(), "TestError");

            // Act
            var result = await _controller.RegisterUser(request);

            var data = (result as BadRequestObjectResult).Value;

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<string>), data);
        }

        [Test]
        public async Task AdminController_RegisterUser_UserExistsWithEmail_ReturnsConflict()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.RegisterUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(ConflictResult), result);
        }

        [Test]
        public async Task AdminController_RegisterUser_ErrorCreatingUser_ReturnsStatusCode()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.CreateUserAsync(A<UserRegistrationRequest>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.RegisterUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task AdminController_RegisterUser_ErrorCreatingUser_Returns500StatusCode()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.CreateUserAsync(A<UserRegistrationRequest>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.RegisterUser(request);

            // Assert
            var statusCode = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, statusCode);
        }

        [Test]
        public async Task AdminController_RegisterUser_CreationSuccess_ReturnsCreated()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.CreateUserAsync(A<UserRegistrationRequest>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.RegisterUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(CreatedResult), result);
        }

        [Test]
        public async Task AdminController_RegisterUser_CreationSuccess_CreatedResponseContainsUserId()
        {
            // Arrange
            var request = A.Dummy<UserRegistrationRequest>();
            var user = A.Dummy<IdentityUser>();
            const string id = "TEST_ID";
            user.Id = id;
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.CreateUserAsync(A<UserRegistrationRequest>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.RegisterUser(request);

            // Assert
            var returnedId = (result as CreatedResult).Location;
            Assert.AreEqual(id, returnedId);
        }

        [Test]
        public async Task AdminController_UpdateUser_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_UpdateUser_InvalidModelState_BadRequestContainsListOfErrors()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            var data = (result as BadRequestObjectResult).Value;

            Assert.IsInstanceOf(typeof(IEnumerable<string>), data);
        }

        [Test]
        public async Task AdminController_UpdateUser_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task AdminController_UpdateUser_UpdateError_ReturnsStatusCodeResult()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            request.Roles = null; // We'll test this later
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task AdminController_UpdateUser_UpdateError_Returns500StatusCode()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            request.Roles = null; // We'll test this later
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            var statusCode = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, statusCode);
        }

        [Test]
        public async Task AdminController_UpdateUser_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var user = A.Dummy<IdentityUser>();
            request.Roles = null;
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .Returns(null);

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task AdminController_UpdateUser_AddsRoles()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var user = A.Dummy<IdentityUser>();
            var roles = new List<string>();
            request.Roles = new List<string>() { "TEST_ROLE" };
            bool roleExists = true;
            bool roleAdded = false;

            // User exists
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);

            // Updates successfully
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .Returns(null);

            // Current roles are empty
            A.CallTo(() => _identityService.GetUserRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);

            // The given role exists
            A.CallTo(() => _identityService.RoleExistsAsync(A<string>.Ignored))
                .Returns(roleExists);
            
            // Set flag when adding role
            A.CallTo(() => _identityService.AddRoleToUserAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Invokes(() => roleAdded = true)
                .Returns(null);

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            Assert.IsTrue(roleAdded);
        }

        [Test]
        public async Task AdminController_UpdateUser_RemovesRoles()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var user = A.Dummy<IdentityUser>();
            var roles = new List<string>() { "TEST_ROLE" };
            request.Roles = new List<string>();
            bool roleExists = true;
            bool roleRemoved = false;

            // User exists
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);

            // Updates successfully
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .Returns(null);

            // Current roles include TEST_ROLE
            A.CallTo(() => _identityService.GetUserRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);

            // The given role exists
            A.CallTo(() => _identityService.RoleExistsAsync(A<string>.Ignored))
                .Returns(roleExists);
            
            // Set flag when removing role
            A.CallTo(() => _identityService.RemoveRoleFromUserAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Invokes(() => roleRemoved = true)
                .Returns(null);

            // Act
            var result = await _controller.UpdateUser(request);

            // Assert
            Assert.IsTrue(roleRemoved);
        }

        [Test]
        public async Task AdminController_DeleteUser_EmptyIdString_ReturnsBadRequest()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_DeleteUser_EmptyIdString_BadRequestContainsErrorMessage()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            var data = (string)(result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_DeleteUser_NoUserMatch_ReturnsNotFound()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task AdminController_DeleteUser_DeleteError_ReturnsStatusCode()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.DeleteUserAsync(A<string>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task AdminController_DeleteUser_DeleteError_Returns500StatusCode()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.DeleteUserAsync(A<string>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task AdminController_DeleteUser_DeletionSuccess_ReturnsNoContent()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.DeleteUserAsync(A<string>.Ignored))
                .Returns(null);

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }

        [Test]
        public async Task AdminController_GetUserById_EmptyIdString_ReturnsBadRequest()
        {
            // Arrange
            var id = string.Empty;
            
            // Act
            var result = await _controller.GetUserById(id);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_GetUserById_EmptyIdString_BadRequestContainsErrorMessage()
        {
            // Arrange
            var id = string.Empty;
            
            // Act
            var result = await _controller.GetUserById(id);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_GetUserById_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.GetUserById(id);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task AdminController_GetUserById_Success_ReturnsOk()
        {
            // Arrange
            var id = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.GetUserById(id);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task AdminController_GetUserById_Success_OkContainsUserData()
        {
            // Arrange
            var id = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.GetUserById(id);

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_GetUserByEmail_EmptyEmailString_ReturnsBadRequest()
        {
            // Arrange
            var email = string.Empty;

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_GetUserByEmail_EmptyEmailString_BadRequestContainsErrorMessage()
        {
            // Arrange
            var email = string.Empty;

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_GetUserByEmail_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            var email = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task AdminController_GetUserByEmail_MatchingUser_ReturnsOk()
        {
            // Arrange
            var email = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserByEmailAsync(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task AdminController_GetUserByEmail_MatchingUser_OkContainsUserData()
        {
            // Arrange
            var email = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserByEmailAsync(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_UpdateLog_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_UpdateLog_InvalidModelState_BadRequestContainsErrorMessages()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_UpdateLog_NoMatchingLog_ReturnsNotFound()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task AdminController_UpdateLog_UpdateError_ReturnsStatusCode()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task AdminController_UpdateLog_UpdateError_Returns500StatusCode()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task AdminController_UpdateLog_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            var request = A.Dummy<LogUpdateRequest>();
            var log = A.Dummy<LogFile>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.UpdateAsync(A<LogFile>.Ignored))
                .Returns(log);

            // Act
            var result = await _controller.UpdateLog(request);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task AdminController_DeleteLog_EmptyIdString_ReturnsBadRequest()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task AdminController_DeleteLog_EmptyIdString_BadRequestContainsErrorMessages()
        {
            // Arrange
            var id = string.Empty;

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task AdminController_DeleteLog_NoMatchingLog_ReturnsNotFound()
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
        public async Task AdminController_DeleteLog_DeletionSuccess_ReturnsNoContent()
        {
            // Arrange
            var id = A.Dummy<string>();
            A.CallTo(() => _logsService.LogExists(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _logsService.DeleteAsync(A<string>.Ignored))
                .Returns(null);

            // Act
            var result = await _controller.DeleteLog(id);

            // Assert
            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }

        #endregion
    }
}
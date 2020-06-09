using System.Collections.Generic;
using System.Threading.Tasks;
using API.Contracts;
using API.Contracts.Requests;
using API.Contracts.Responses;
using API.Controllers.V2;
using API.Exceptions;
using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace api.test
{
    public class IdentityControllerV2Tests
    {
        private IdentityController _controller;
        private IIdentityService _identityService;

        [SetUp]
        public void Setup()
        {
            _identityService = A.Fake<IIdentityService>();
            _controller = new IdentityController(_identityService);
        }

        [Test]
        public async Task IdentityControllerV2_Get_NoAuthorizationHeader_ReturnsUnauthorized()
        {
            // Arrange
            var token = string.Empty;

            // Act
            var result = await _controller.Get(token);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Get_InvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            var token = A.Dummy<string>();

            A.CallTo(() => _identityService.ValidateTokenAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.Get(token);

            // Assert
            Assert.IsInstanceOf(typeof(UnauthorizedResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Get_NoUserMatch_ReturnsNotFound()
        {
            // Arrange
            var token = A.Dummy<string>();

            A.CallTo(() => _identityService.ValidateTokenAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.Get(token);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Get_ValidToken_ReturnsOk()
        {
            // Arrange
            var token = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();

            A.CallTo(() => _identityService.ValidateTokenAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.Get(token);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Get_ValidToken_OkResultContainsUserData()
        {
            // Arrange
            var token = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();

            A.CallTo(() => _identityService.ValidateTokenAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.Get(token);

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsInstanceOf(typeof(UserDataResponse), data);
        }

        [Test]
        public async Task IdentityControllerV2_Login_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Login_InvalidModelState_BadRequestContainsErrorMessages()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Login(request);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsInstanceOf(typeof(SerializableError), data);
        }

        [Test]
        public async Task IdentityControllerV2_Login_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Login_FailedLoginAttempt_ReturnsUnauthorized()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.LoginAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns((string)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf(typeof(UnauthorizedResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Login_LoginSuccess_ReturnsOk()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            var token = A.Dummy<string>();
            var roles = new List<string>
            {
                "User"
            };

            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);
            A.CallTo(() => _identityService.LoginAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(token);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Login_LoginSuccess_OkResultContainsTokenResponse()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            const string token = "THIS_IS_A_TOKEN";
            var roles = new List<string>
            {
                "User"
            };

            A.CallTo(() => _identityService.LoginAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(token);
            A.CallTo(() => _identityService.GetUserRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);
            A.CallTo(() => _identityService.UserExistsWithEmailAsync(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsInstanceOf(typeof(TokenResponse), data);
        }

        [Test]
        public async Task IdentityControllerV2_ChangePassword_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            string jwt = A.Dummy<string>();
            PasswordChangeRequest request = A.Dummy<PasswordChangeRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.ChangePassword(request, jwt);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_ChangePassword_InvalidModelState_BadRequestContainsErrors()
        {
            // Arrange
            string jwt = A.Dummy<string>();
            PasswordChangeRequest request = A.Dummy<PasswordChangeRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.ChangePassword(request, jwt);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsInstanceOf(typeof(SerializableError), data);
        }

        [Test]
        public async Task IdentityControllerV2_ChangePassword_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            string jwt = A.Dummy<string>();
            PasswordChangeRequest request = A.Dummy<PasswordChangeRequest>();
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.ChangePassword(request, jwt);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_ChangePassword_ActionFailed_ReturnsStatusCode()
        {
            // Arrange
            string jwt = A.Dummy<string>();
            PasswordChangeRequest request = A.Dummy<PasswordChangeRequest>();
            IdentityUser user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns(user);
            A.CallTo(() => _identityService.SetUserPasswordAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.ChangePassword(request, jwt);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_ChangePassword_ActionFailed_Returns500StatusCode()
        {
            // Arrange
            string jwt = A.Dummy<string>();
            PasswordChangeRequest request = A.Dummy<PasswordChangeRequest>();
            IdentityUser user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns(user);
            A.CallTo(() => _identityService.SetUserPasswordAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.ChangePassword(request, jwt);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task IdentityControllerV2_ChangePassword_Success_ReturnsOk()
        {
            // Arrange
            string jwt = A.Dummy<string>();
            PasswordChangeRequest request = A.Dummy<PasswordChangeRequest>();
            IdentityUser user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.ChangePassword(request, jwt);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var id = A.Dummy<string>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_InvalidModelState_BadRequestResultContainsErrorMessages()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var id = A.Dummy<string>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task IdentityControllerV2_Update_EmptyIdString_ReturnsBadRequest()
        {
            // Arrange
            var id = string.Empty;
            var request = A.Dummy<UserUpdateRequest>();

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_UpdateFailure_ReturnsStatusCodeResult()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var user = A.Dummy<IdentityUser>();
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(user);
            A.CallTo(() => _identityService.UpdateUserAsync(A<IdentityUser>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_UpdateFailure_Returns500StatusCode()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var id = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(user);
            A.CallTo(() => _identityService.UpdateUserAsync(A<IdentityUser>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task IdentityControllerV2_Update_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            var id = A.Dummy<string>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }
    }
}
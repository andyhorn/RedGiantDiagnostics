using System.Threading.Tasks;
using API.Contracts;
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
        private IdentityControllerV2 _controller;
        private IIdentityService _identityService;

        [SetUp]
        public void Setup()
        {
            _identityService = A.Fake<IIdentityService>();
            _controller = new IdentityControllerV2(_identityService);
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
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task IdentityControllerV2_Get_Login_InvalidModelState_ReturnsBadRequest()
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
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task IdentityControllerV2_Login_FailedLoginAttempt_ReturnsUnauthorized()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
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
            A.CallTo(() => _identityService.LoginAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(token);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Login_LoginSuccess_OkResultContainsToken()
        {
            // Arrange
            var request = A.Dummy<UserLoginRequest>();
            const string token = "THIS_IS_A_TOKEN";
            A.CallTo(() => _identityService.LoginAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(token);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var data = (string)(result as OkObjectResult).Value;
            Assert.AreEqual(token, data);
        }

        [Test]
        public async Task IdentityControllerV2_Update_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Update(request);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_InvalidModelState_BadRequestResultContainsErrorMessages()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            _controller.ModelState.AddModelError(string.Empty, "TEST_ERROR");

            // Act
            var result = await _controller.Update(request);

            // Assert
            var data = (result as BadRequestObjectResult).Value;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task IdentityControllerV2_Update_NoMatchingUser_ReturnsNotFound()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _controller.Update(request);

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_UpdateFailure_ReturnsStatusCodeResult()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.Update(request);

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Update_UpdateFailure_Returns500StatusCode()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _identityService.UpdateUserAsync(A<UserUpdateRequest>.Ignored))
                .ThrowsAsync(new ActionFailedException());

            // Act
            var result = await _controller.Update(request);

            // Assert
            var code = (result as StatusCodeResult).StatusCode;
            Assert.AreEqual(500, code);
        }

        [Test]
        public async Task IdentityControllerV2_Update_UpdateSuccess_ReturnsOk()
        {
            // Arrange
            var request = A.Dummy<UserUpdateRequest>();
            A.CallTo(() => _identityService.UserExistsWithIdAsync(A<string>.Ignored))
                .Returns(true);
            
            // Act
            var result = await _controller.Update(request);

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }
    }
}
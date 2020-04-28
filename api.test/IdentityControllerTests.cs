using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
using API.Controllers;
using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace api.test
{
    public class IdentityControllerTests
    {
        private IIdentityService _identityService;
        private IdentityController _controller;

        [SetUp]
        public void Setup()
        {
            _identityService = A.Fake<IIdentityService>();
            _controller = new IdentityController(_identityService);
        }

        [Test]
        public async Task IdentityController_GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange
            const int numUsers = 5;
            A.CallTo(() => _identityService.GetAllUsersAsync())
                .Returns(A.CollectionOfFake<IdentityUser>(5));

            // Act
            var obj = await _controller.GetAllUsersAsync();
            var result = obj as OkObjectResult;
            var data = result.Value as IEnumerable<IdentityUser>;

            // Assert
            Assert.AreEqual(numUsers, data.Count());
        }

        [Test]
        public async Task IdentityController_GetUserById_NullIdReturnsBadRequest()
        {
            // Arrange
            
            // Act
            var result = await _controller.GetUserByIdAsync(null);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task IdentityController_GetUserById_EmptyIdReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _controller.GetUserByIdAsync(string.Empty);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task IdentityController_GetUserById_InvalidIdReturnsNotFound()
        {
            // Arrange
            IdentityUser nullUser = null;
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(nullUser);

            // Act
            var result = await _controller.GetUserByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityController_GetUserById_ValidIdReturnsUserObject()
        {
            // Arrange
            const string id = "FAKE_ID";
            var user = A.Fake<IdentityUser>();
            user.Id = id;

            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _controller.GetUserByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);

            var obj = result as OkObjectResult;

            Assert.IsInstanceOf(typeof(IdentityUser), obj.Value);
            Assert.AreEqual(id, (obj.Value as IdentityUser).Id);
        }

        [Test]
        public async Task IdentityController_GetUserByEmail_EmptyEmailReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _controller.GetUserByEmail(string.Empty);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityController_GetUserByEmail_InvalidEmailReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByEmailAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.GetUserByEmail(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityController_GetUserByEmail_ValidEmailReturnsOk()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByEmailAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _controller.GetUserByEmail(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public async Task IdentityController_GetUserByEmail_OkResultContainsUserObject()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByEmailAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _controller.GetUserByEmail(A.Dummy<string>());

            // Assert
            var data = (result as OkObjectResult).Value;
            Assert.IsInstanceOf(typeof(IdentityUser), data);
        }

        [Test]
        public async Task IdentityController_CreateUser_NullBodyReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError(string.Empty, "Request body cannot be empty.");

            // Act
            var result = await _controller.CreateUserAsync(null);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestResult), result);
        }

        [Test]
        public async Task IdentityController_CreateUser_ValidRequestReturnsCreatedResult()
        {
            // Arrange
            A.CallTo(() => _identityService.CreateUserAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _controller.CreateUserAsync(A.Fake<RegisterUserRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(CreatedResult), result);
        }

        [Test]
        public async Task IdentityController_CreateUser_CreatedResultContainsUserId()
        {
            // Arrange
            A.CallTo(() => _identityService.CreateUserAsync(A<string>.Ignored, A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _controller.CreateUserAsync(A.Fake<RegisterUserRequest>());

            // Assert
            var data = (result as CreatedResult).Location;
            Assert.IsNotNull(data);
        }

        [Test]
        public async Task IdentityController_UpdateUser_NullOrEmptyIdReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _controller.UpdateUserAsync(string.Empty, A.Fake<UpdateUserRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityController_UpdateUser_InvalidModelStateReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError(string.Empty, "Test Error");

            // Act
            var result = await _controller.UpdateUserAsync(A.Dummy<string>(), A.Fake<UpdateUserRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityController_UpdateUser_InvalidIdReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.UpdateUserAsync(A.Dummy<string>(), A.Fake<UpdateUserRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityController_UpdateUser_UpdatedUserReturnsOk()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            A.CallTo(() => _identityService.UpdateUserAsync(A<IdentityUser>.Ignored))
                .DoesNothing();

            // Act
            var result = await _controller.UpdateUserAsync(A.Dummy<string>(), A.Fake<UpdateUserRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        [Test]
        public async Task IdentityController_UpdateUser_HandlesException_ReturnsServerError()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            A.CallTo(() => _identityService.UpdateUserAsync(A<IdentityUser>.Ignored))
                .Throws(new Exception());

            // Act
            var result = await _controller.UpdateUserAsync(A.Dummy<string>(), A.Fake<UpdateUserRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
            Assert.AreEqual(500, (result as StatusCodeResult).StatusCode);
        }

        [Test]
        public async Task IdentityController_DeleteUser_EmptyIdReturnsBadRequest()
        {
            // Arrange

            // Act
            var result = await _controller.DeleteUserAsync(string.Empty);

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public async Task IdentityController_DeleteUser_InvalidIdReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.DeleteUserAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }

        [Test]
        public async Task IdentityController_DeleteUser_DeleteErrorReturnsServerError()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            A.CallTo(() => _identityService.DeleteUserAsync(A<string>.Ignored))
                .Throws(new Exception());

            // Act
            var result = await _controller.DeleteUserAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(StatusCodeResult), result);
            Assert.AreEqual(500, (result as StatusCodeResult).StatusCode);
        }

        [Test]
        public async Task IdentityController_DeleteUser_ReturnsNoContentResult()
        {
            // Arrange
            A.CallTo(() => _identityService.GetUserByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            A.CallTo(() => _identityService.DeleteUserAsync(A<string>.Ignored))
                .DoesNothing();

            // Act
            var result = await _controller.DeleteUserAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(NoContentResult), result);
        }

        [Test]
        public void IdentityController_Login_InvalidModelStateReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError(string.Empty, "Test Error");

            // Act
            var result = _controller.Login(A.Fake<UserLoginRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
        }

        [Test]
        public void IdentityController_Login_ReturnsOkResult()
        {
            // Arrange
            A.CallTo(() => _identityService.Login(A<string>.Ignored, A<string>.Ignored))
                .Returns(A.Dummy<string>());

            // Act
            var result = _controller.Login(A.Fake<UserLoginRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
        }

        [Test]
        public void IdentityController_Login_OkResultContainsToken()
        {
            // Arrange
            A.CallTo(() => _identityService.Login(A<string>.Ignored, A<string>.Ignored))
                .Returns(A.Dummy<string>());

            // Act
            var result = _controller.Login(A.Fake<UserLoginRequest>());

            // Assert
            Assert.IsNotNull((string)(result as OkObjectResult).Value);
        }

        [Test]
        public void IdentityController_Login_InvalidLoginReturnsUnauthorized()
        {
            // Arrange
            A.CallTo(() => _identityService.Login(A<string>.Ignored, A<string>.Ignored))
                .Returns(null);

            // Act
            var result = _controller.Login(A.Fake<UserLoginRequest>());

            // Assert
            Assert.IsInstanceOf(typeof(UnauthorizedResult), result);
        }
    }
}
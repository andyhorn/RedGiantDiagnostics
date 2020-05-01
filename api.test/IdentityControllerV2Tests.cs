using System.Threading.Tasks;
using API.Controllers.V2;
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

            A.CallTo(() => _identityService.GetUserFromToken(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _controller.Get(token);

            // Assert
            Assert.IsInstanceOf(typeof(UnauthorizedResult), result);
        }

        [Test]
        public async Task IdentityControllerV2_Get_ExpiredToken_ReturnsUnauthorized()
        {
            // Arrange
            var token = A.Dummy<string>();
            // A.CallTo(() => _identityService.)
        }
    }
}
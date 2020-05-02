using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace api.test
{
    public class TokenServiceTests
    {
        private UserManager<IdentityUser> _userManager;
        private ITokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("TOKEN_SECRET", "ThisIsALongStringForTestingTheSecurityKey");
            _userManager = A.Fake<UserManager<IdentityUser>>();
            _tokenService = new TokenService(_userManager);
        }

        [Test]
        public void TokenService_MakeToken_NullUserThrowsException()
        {
            // Arrange

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _tokenService.MakeToken(null));
        }

        [Test]
        public async Task TokenService_MakeToken_ValidUserGeneratesToken()
        {
            // Arrange
            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(A.CollectionOfDummy<string>(3));
            var user = A.Dummy<IdentityUser>();

            // Act
            var result = await _tokenService.MakeToken(user);

            // Assert
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void TokenService_GetUserId_EmptyStringThrowsException()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _tokenService.GetUserId(string.Empty));
        }

        [Test]
        public async Task TokenService_GetUserId_RetrievesUserIdFromValidToken()
        {
            // Arrange
            const string id = "TestUserId";
            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(A.CollectionOfDummy<string>(3));
            var user = A.Dummy<IdentityUser>();
            user.Id = id;

            var token = await _tokenService.MakeToken(user);

            // Act
            var userId = _tokenService.GetUserId(token);

            // Assert
            Assert.AreEqual(id, userId);
        }

        [Test]
        public void TokenService_IsValid_EmptyTokenString_ThrowsException()
        {
            // Arrange
            var token = string.Empty;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _tokenService.IsValid(token));
        }

        [Test]
        public void TokenService_IsValid_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var token = A.Dummy<string>();

            // Act
            var result = _tokenService.IsValid(token);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
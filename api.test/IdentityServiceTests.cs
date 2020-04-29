using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace api.test
{
    public class IdentityServiceTests
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private ITokenService _tokenService;
        private IIdentityService _identityService;

        [SetUp]
        public void Setup()
        {
            _userManager = A.Fake<UserManager<IdentityUser>>();
            _signInManager = A.Fake<SignInManager<IdentityUser>>();
            _roleManager = A.Fake<RoleManager<IdentityRole>>();
            _tokenService = A.Fake<ITokenService>();

            _identityService = new IdentityService(
                _userManager,
                _signInManager,
                _roleManager,
                _tokenService
            );
        }

        [Test]
        public void IdentityService_CreateUserAsync_EmptyEmailThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.CreateUserAsync(string.Empty, A.Dummy<string>()));
        }

        [Test]
        public void IdentityService_CreateUserAsync_EmptyPasswordThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.CreateUserAsync(A.Dummy<string>(), string.Empty));
        }

        [Test]
        public void IdentityService_CreateUserAsync_ExistingUser_ThrowsException()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            Assert.ThrowsAsync<ResourceConflictException>(() => _identityService.CreateUserAsync(A.Dummy<string>(), A.Dummy<string>()));
        }

        [Test]
        public async Task IdentityService_CreateUserAsync_ValidParameters_CreatesNewUser()
        {
            // Arrange
            var identityResult = IdentityResult.Success;
            
            IdentityUser[] identityUserResults = new IdentityUser[]
            {
                null,
                A.Fake<IdentityUser>()
            };

            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .ReturnsNextFromSequence(identityUserResults);
            A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(identityResult);

            // Act
            var result = await _identityService.CreateUserAsync(A.Dummy<string>(), A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(IdentityUser), result);
        }

        [Test]
        public void IdentityService_DeleteUserAsync_EmptyIdThrowsException()
        {
            // Arrange

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.DeleteUserAsync(string.Empty));
        }

        [Test]
        public void IdentityService_DeleteUserAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _identityService.DeleteUserAsync(A.Dummy<string>()));
        }

        [Test]
        public async Task IdentityService_DeleteUserAsync_DeletesUserObject()
        {
            // Arrange
            bool deleted = false;
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _userManager.DeleteAsync(A<IdentityUser>.Ignored))
                .Invokes(() => deleted = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.DeleteUserAsync(A.Dummy<string>());

            // Assert
            Assert.IsTrue(deleted);
        }

        [Test]
        public async Task IdentityService_GetAllUsers_ReturnsListOfIdentityUsers()
        {
            // Arrange
            const int numUsers = 3;
            var list = new List<IdentityUser>();
            for (var i = 0; i < numUsers; i++)
            {
                list.Add(A.Fake<IdentityUser>());
            }
            A.CallTo(() => _userManager.Users).Returns(list.AsQueryable());

            // Act
            var result = await _identityService.GetAllUsersAsync();

            // Assert
            Assert.IsInstanceOf(typeof(IQueryable<IdentityUser>), result);
            Assert.AreEqual(numUsers, result.Count());
        }

        [Test]
        public void IdentityService_GetUserById_EmptyIdThrowsException()
        {
            // Arrange

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetUserByIdAsync(string.Empty));
        }

        [Test]
        public async Task IdentityService_GetUserById_InvalidId_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.GetUserByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task IdentityService_GetUserById_ValidId_ReturnsIdentityUser()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _identityService.GetUserByIdAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(IdentityUser), result);
        }

        [Test]
        public void IdentityService_GetUserByEmailAsync_EmptyEmailThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetUserByEmailAsync(string.Empty));
        }

        [Test]
        public async Task IdentityService_GetUserByEmailAsync_InvalidEmail_ReturnsNull()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.GetUserByEmailAsync(A.Dummy<string>());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task IdentityService_GetUserByEmailAsync_ValidEmail_ReturnsIdentityUser()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _identityService.GetUserByEmailAsync(A.Dummy<string>());

            // Assert
            Assert.IsInstanceOf(typeof(IdentityUser), result);
        }

        [Test]
        public void IdentityService_UpdateUserAsync_UpdateError_ThrowsException()
        {
            // Arrange
            var error = new IdentityError();
            var identityResult = IdentityResult.Failed(error);
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _userManager.UpdateAsync(A<IdentityUser>.Ignored))
                .Returns(identityResult);

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _identityService.UpdateUserAsync(A.Fake<IdentityUser>()));
        }

        [Test]
        public async Task IdentityService_UpdateUserAsync_ValidParameters_Succeeds()
        {
            // Arrange
            bool updated = false;
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _userManager.UpdateAsync(A<IdentityUser>.Ignored))
                .Invokes(() => updated = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.UpdateUserAsync(A.Fake<IdentityUser>());

            // Assert
            Assert.IsTrue(updated);
        }

        [Test]
        public void IdentityService_Login_EmptyEmailThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.LoginAsync(string.Empty, A.Dummy<string>()));
        }

        [Test]
        public void IdentityService_Login_EmptyPasswordThrowsException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.LoginAsync(A.Dummy<string>(), string.Empty));
        }

        [Test]
        public void IdentityService_Login_InvalidEmail_ThrowsException()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act and Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _identityService.LoginAsync(A.Dummy<string>(), A.Dummy<string>()));
        }

        [Test]
        public void IdentityService_Login_LoginFails_ThrowsException()
        {
            // Arrange
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _signInManager.PasswordSignInAsync(
                A<string>.Ignored, 
                A<string>.Ignored,
                A<bool>.Ignored,
                A<bool>.Ignored
                ))
                .Returns(SignInResult.Failed);

            Assert.ThrowsAsync<ArgumentException>(() => _identityService.LoginAsync(A.Dummy<string>(), A.Dummy<string>()));
        }

        [Test]
        public async Task IdentityService_Login_LoginSuccess_ReturnsToken()
        {
            // Arrange
            A.CallTo(() => 
                _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => 
                _signInManager.PasswordSignInAsync(
                    A<string>.Ignored,
                    A<string>.Ignored,
                    A<bool>.Ignored,
                    A<bool>.Ignored
                ))
                .Returns(SignInResult.Success);
            A.CallTo(() =>
                _tokenService.MakeToken(A<IdentityUser>.Ignored))
                .Returns(A.Dummy<string>());

            // Act
            var result = await _identityService.LoginAsync(A.Dummy<string>(), A.Dummy<string>());

            // Assert
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void IdentityService_GetUserFromToken_EmptyTokenStringThrowsException()
        {
            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetUserFromToken(string.Empty));
        }

        [Test]
        public void IdentityService_GetUserFromToken_InvalidTokenThrowsException()
        {
            // Arrange
            var token = A.Dummy<string>();
            A.CallTo(() => _tokenService.GetUserId(A<string>.Ignored))
                .Returns(string.Empty);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _identityService.GetUserFromToken(token));
        }

        [Test]
        public async Task IdentityService_GetUserFromToken_NullUserReturnsNull()
        {
            // Arrange
            var token = A.Dummy<string>();
            var userId = A.Dummy<string>();
            A.CallTo(() => _tokenService.GetUserId(A<string>.Ignored))
                .Returns(userId);
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.GetUserFromToken(token);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task IdentityService_GetUserFromToken_ValidTokenWithUserId_ReturnsUserObject()
        {
            // Arrange
            var token = A.Dummy<string>();
            var userId = A.Dummy<string>();
            var user = A.Dummy<IdentityUser>();
            A.CallTo(() => _tokenService.GetUserId(A<string>.Ignored))
                .Returns(userId);
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(user);

            // Act
            var result = await _identityService.GetUserFromToken(token);

            // Assert
            Assert.IsInstanceOf(typeof(IdentityUser), result);
        }
    }
}
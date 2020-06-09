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
            // Arrange
            var newUser = A.Fake<IdentityUser>();
            newUser.Email = null;
            var password = A.Dummy<string>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.CreateUserAsync(newUser, password));
        }

        [Test]
        public void IdentityService_CreateUserAsync_EmptyPasswordThrowsException()
        {
            // Arrange
            var newUser = A.Fake<IdentityUser>();
            var password = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.CreateUserAsync(newUser, password));
        }

        [Test]
        public void IdentityService_CreateUserAsync_ExistingUser_ThrowsException()
        {
            // Arrange
            var newUser = A.Dummy<IdentityUser>();
            var password = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            Assert.ThrowsAsync<ResourceConflictException>(() => _identityService.CreateUserAsync(newUser, password));
        }

        [Test]
        public async Task IdentityService_CreateUserAsync_ValidParameters_CreatesNewUser()
        {
            // Arrange
            var identityResult = IdentityResult.Success;
            var user = A.Dummy<IdentityUser>();
            var password = A.Dummy<string>();
            IdentityUser[] identityUserResults = new IdentityUser[]
            {
                null,
                A.Fake<IdentityUser>()
            };

            // User doesn't exist on first check, but exists after creation
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .ReturnsNextFromSequence(identityUserResults);

            // IdentityRole exists
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);

            // User creation succeeds
            A.CallTo(() => _userManager.CreateAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(identityResult);

            // Adding user to role succeeds
            A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Success);

            // Act
            var result = await _identityService.CreateUserAsync(user, password);

            // Assert
            Assert.IsInstanceOf(typeof(IdentityUser), result);
        }

        [Test]
        public void IdentityService_DeleteUserAsync_EmptyIdThrowsException()
        {
            // Arrange
            string id = string.Empty;

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.DeleteUserAsync(id));
        }

        [Test]
        public void IdentityService_DeleteUserAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            string id = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            Assert.ThrowsAsync<ResourceNotFoundException>(() => _identityService.DeleteUserAsync(id));
        }

        [Test]
        public async Task IdentityService_DeleteUserAsync_DeletesUserObject()
        {
            // Arrange
            bool deleted = false;
            string id = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _userManager.DeleteAsync(A<IdentityUser>.Ignored))
                .Invokes(() => deleted = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.DeleteUserAsync(id);

            // Assert
            Assert.IsTrue(deleted);
        }

        [Test]
        public void IdentityService_GetAllUsers_ReturnsListOfIdentityUsers()
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
            var result = _identityService.GetAllUsers();

            // Assert
            Assert.IsInstanceOf(typeof(IQueryable<IdentityUser>), result);
            Assert.AreEqual(numUsers, result.Count());
        }

        [Test]
        public void IdentityService_GetUserById_EmptyIdThrowsException()
        {
            // Arrange
            string id = string.Empty;

            // Act
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetUserByIdAsync(id));
        }

        [Test]
        public async Task IdentityService_GetUserById_InvalidId_ReturnsNull()
        {
            // Arrange
            string id = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.GetUserByIdAsync(id);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task IdentityService_GetUserById_ValidId_ReturnsIdentityUser()
        {
            // Arrange
            string id = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _identityService.GetUserByIdAsync(id);

            // Assert
            Assert.IsInstanceOf(typeof(IdentityUser), result);
        }

        [Test]
        public void IdentityService_GetUserByEmailAsync_EmptyEmailThrowsException()
        {
            // Arrange
            string id = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetUserByEmailAsync(id));
        }

        [Test]
        public async Task IdentityService_GetUserByEmailAsync_InvalidEmail_ReturnsNull()
        {
            // Arrange
            string email = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.GetUserByEmailAsync(email);

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
            var update = A.Dummy<IdentityUser>();
            var error = new IdentityError();
            var identityResult = IdentityResult.Failed(error);
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _userManager.UpdateAsync(A<IdentityUser>.Ignored))
                .Returns(identityResult);

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _identityService.UpdateUserAsync(update));
        }

        [Test]
        public async Task IdentityService_UpdateUserAsync_ValidParameters_Succeeds()
        {
            // Arrange
            var update = A.Dummy<IdentityUser>();
            bool updated = false;
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());
            A.CallTo(() => _userManager.UpdateAsync(A<IdentityUser>.Ignored))
                .Invokes(() => updated = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.UpdateUserAsync(update);

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
        public void IdentityService_SetUserPassword_NullUserObject_ThrowsException()
        {
            // Arrange
            IdentityUser user = null;
            string password = A.Dummy<string>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.SetUserPasswordAsync(user, password));
        }

        [Test]
        public void IdentityService_SetUserPassword_EmptyPasswordString_ThrowsException()
        {
            // Arrange
            IdentityUser user = A.Dummy<IdentityUser>();
            string password = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.SetUserPasswordAsync(user, password));
        }

        [Test]
        public void IdentityService_SetUserPassword_RemovePassword_ActionFails_ThrowsException()
        {
            // Arrange
            IdentityUser user = A.Dummy<IdentityUser>();
            const string password = "Th1s1s@v@l1dP@ssw0rd!";
            A.CallTo(() => _userManager.HasPasswordAsync(A<IdentityUser>.Ignored))
                .Returns(true);
            A.CallTo(() => _userManager.RemovePasswordAsync(A<IdentityUser>.Ignored))
                .Returns(IdentityResult.Failed(A.Dummy<IdentityError>()));

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _identityService.SetUserPasswordAsync(user, password));
        }

        [Test]
        public void IdentityService_SetUserPassword_AddPassword_ActionFails_ThrowsException()
        {
            // Arrange
            IdentityUser user = A.Dummy<IdentityUser>();
            const string password = "Th1s1s@v@l1dP@ssw0rd!";
            A.CallTo(() => _userManager.HasPasswordAsync(A<IdentityUser>.Ignored))
                .Returns(true);
            A.CallTo(() => _userManager.RemovePasswordAsync(A<IdentityUser>.Ignored))
                .Returns(IdentityResult.Success);
            A.CallTo(() => _userManager.AddPasswordAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Failed());

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _identityService.SetUserPasswordAsync(user, password));
        }

        [Test]
        public async Task IdentityService_SetUserPassword_Success()
        {
            // Arrange
            IdentityUser user = A.Dummy<IdentityUser>();
            const string password = "Th1s1s@v@l1dP@ssw0rd!";
            bool passwordSet = false;
            A.CallTo(() => _userManager.HasPasswordAsync(A<IdentityUser>.Ignored))
                .Returns(true);
            A.CallTo(() => _userManager.RemovePasswordAsync(A<IdentityUser>.Ignored))
                .Returns(IdentityResult.Success);
            A.CallTo(() => _userManager.AddPasswordAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Invokes(() => passwordSet = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.SetUserPasswordAsync(user, password);

            // Assert
            Assert.IsTrue(passwordSet);
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

        [Test]
        public void IdentityService_UserExistsWithIdAsync_EmptyStringThrowsException()
        {
            // Arrange
            string id = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.UserExistsWithIdAsync(id));
        }

        [Test]
        public async Task IdentityService_UserExistsWithIdAsync_NoUserReturnsFalse()
        {
            // Arrange
            string id = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.UserExistsWithIdAsync(id);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IdentityService_UserExistsWithIdAsync_MatchingUserReturnsTrue()
        {
            // Arrange
            string id = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByIdAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _identityService.UserExistsWithIdAsync(id);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IdentityService_UserExistsWithEmailAsync_EmptyStringThrowsException()
        {
            // Arrange
            string email = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.UserExistsWithEmailAsync(email));
        }

        [Test]
        public async Task IdentityService_UserExistsWithEmailAsync_NoMatchReturnsFalse()
        {
            // Arrange
            string email = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns((IdentityUser)null);

            // Act
            var result = await _identityService.UserExistsWithEmailAsync(email);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IdentityService_UserExistsWithEmailAsync_MatchingUserReturnsTrue()
        {
            // Arrange
            string email = A.Dummy<string>();
            A.CallTo(() => _userManager.FindByEmailAsync(A<string>.Ignored))
                .Returns(A.Fake<IdentityUser>());

            // Act
            var result = await _identityService.UserExistsWithEmailAsync(email);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IdentityService_ValidateToken_EmptyTokenString_ThrowsException()
        {
            // Arrange
            var token = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.ValidateTokenAsync(token));
        }

        [Test]
        public async Task IdentityService_ValidateToken_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var token = A.Dummy<string>();
            A.CallTo(() => _tokenService.IsValid(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _identityService.ValidateTokenAsync(token);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IdentityService_ValidateToken_ValidToken_ReturnsTrue()
        {
            // Arrange
            var token = A.Dummy<string>();
            A.CallTo(() => _tokenService.IsValid(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _identityService.ValidateTokenAsync(token);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IdentityService_SetUserRoles_NullUserObject_ThrowsException()
        {
            // Arrange
            IdentityUser user = null;
            var roles = A.CollectionOfDummy<string>(3);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.SetUserRolesAsync(user, roles));
        }

        [Test]
        public void IdentityService_SetUserRoles_NullRolesList_ThrowsException()
        {
            // Arrange
            IdentityUser user = A.Dummy<IdentityUser>();
            List<string> roles = null;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.SetUserRolesAsync(user, roles));
        }

        [Test]
        public void IdentityService_SetUserRoles_RoleDoesntExist_ThrowsException()
        {
            // Arrange
            var user = A.Dummy<IdentityUser>();
            var roles = A.CollectionOfDummy<string>(3);
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(false);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _identityService.SetUserRolesAsync(user, roles));
        }

        [Test]
        public void IdentityService_SetUserRoles_RemoveRoleFails_ThrowsException()
        {
            // Arrange
            var user = A.Dummy<IdentityUser>();
            var setRoles = new List<string>
            {
                "RoleOne"
            };
            var currentRoles = new List<string>
            {
                "RoleOne",
                "RoleTwo"
            };
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(currentRoles);
            A.CallTo(() => _userManager.RemoveFromRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Failed());

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _identityService.SetUserRolesAsync(user, setRoles));
        }

        [Test]
        public void IdentityService_SetUserRoles_AddRoleFails_ThrowsException()
        {
            // Arrange
            var user = A.Dummy<IdentityUser>();
            var setRoles = new List<string>
            {
                "RoleOne",
                "RoleTwo"
            };
            var currentRoles = new List<string>
            {
                "RoleOne"
            };
            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(currentRoles);
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Failed());

            // Act and Assert
            Assert.ThrowsAsync<ActionFailedException>(() => _identityService.SetUserRolesAsync(user, setRoles));
        }

        [Test]
        public async Task IdentityService_SetUserRoles_Success()
        {
            // Arrange
            var user = A.Dummy<IdentityUser>();
            var success = false;
            var setRoles = new List<string>
            {
                "RoleOne",
                "RoleTwo"
            };
            var currentRoles = new List<string>
            {
                "RoleOne"
            };
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(currentRoles);
            A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Invokes(() => success = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.SetUserRolesAsync(user, setRoles);

            // Assert
            Assert.IsTrue(success);
        }

        [Test]
        public void IdentityService_RoleExistsAsync_EmptyStringThrowsException()
        {
            // Arrange
            string role = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.RoleExistsAsync(role));
        }

        [Test]
        public async Task IdentityService_RoleExistsAsync_NoRoleReturnsFalse()
        {
            // Arrange
            var role = A.Dummy<string>();
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _identityService.RoleExistsAsync(role);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IdentityService_RoleExistsAsync_ExistingRoleReturnsTrue()
        {
            // Arrange
            var role = A.Dummy<string>();
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);

            // Act
            var result = await _identityService.RoleExistsAsync(role);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IdentityService_GetRoleAsync_EmptyStringThrowsException()
        {
            // Arrange
            var role = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetRoleAsync(role));
        }

        [Test]
        public async Task IdentityService_GetRoleAsync_NoMatchReturnsNull()
        {
            // Arrange
            var role = A.Dummy<string>();

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            var result = await _identityService.GetRoleAsync(role);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task IdentityService_GetRolesAsync_MatchingRoleReturnsIdentityRole()
        {
            // Arrange
            var roleName = A.Dummy<string>();
            var role = A.Fake<IdentityRole>();

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _roleManager.FindByNameAsync(A<string>.Ignored))
                .Returns(role);

            // Act
            var result = await _identityService.GetRoleAsync(roleName);

            // Assert
            Assert.IsInstanceOf(typeof(IdentityRole), result);
        }

        [Test]
        public void IdentityService_CreateRoleAsync_EmptyStringThrowsException()
        {
            // Arrange
            var role = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.CreateRoleAsync(role));
        }

        [Test]
        public async Task IdentityService_CreateRoleAsync_ExistingRoleFailsSilently()
        {
            // Arrange
            var role = A.Dummy<string>();

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);

            // Act
            await _identityService.CreateRoleAsync(role);

            // Assert
            A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task IdentityService_CreateRoleAsync_NoExistingRoleCreatesRole()
        {
            // Arrange
            var role = A.Dummy<string>();
            bool created = false;

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(false);
            A.CallTo(() => _roleManager.CreateAsync(A<IdentityRole>.Ignored))
                .Invokes(() => created = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.CreateRoleAsync(role);

            // Assert
            Assert.IsTrue(created);
        }

        [Test]
        public async Task IdentityService_DeleteRoleAsync_NoExistingRoleFailsSilently()
        {
            // Arrange
            var role = A.Dummy<string>();

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(false);

            // Act
            await _identityService.DeleteRoleAsync(role);

            // Assert
            A.CallTo(() => _roleManager.FindByNameAsync(A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _roleManager.DeleteAsync(A<IdentityRole>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task IdentityService_DeleteRoleAsync_MatchingRoleDeletes()
        {
            // Arrange
            var role = A.Dummy<string>();
            var fakeRole = A.Fake<IdentityRole>();
            bool deleted = false;

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);
            A.CallTo(() => _roleManager.FindByNameAsync(A<string>.Ignored))
                .Returns(fakeRole);
            A.CallTo(() => _roleManager.DeleteAsync(A<IdentityRole>.Ignored))
                .Invokes(() => deleted = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.DeleteRoleAsync(role);

            // Assert
            Assert.IsTrue(deleted);
        }

        [Test]
        public void IdentityService_AddRoleToUser_NullUserThrowsException()
        {
            // Arrange
            var role = A.Dummy<string>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.AddRoleToUserAsync(null, role));
        }

        [Test]
        public void IdentityService_AddRoleToUser_NullRoleThrowsException()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.AddRoleToUserAsync(user, null));
        }

        [Test]
        public void IdentityService_AddRoleToUser_InvalidRoleNameThrowsException()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            var role = A.Dummy<string>();
            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(false);

            // Act and Assert
            Assert.ThrowsAsync<ArgumentException>(() => _identityService.AddRoleToUserAsync(user, role));
        }

        [Test]
        public async Task IdentityService_AddRoleToUser_AddsRoleToUserObject()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            var roleName = A.Dummy<string>();

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);

            A.CallTo(() => _userManager.AddToRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.AddRoleToUserAsync(user, roleName);

            // Assert
            Assert.Pass();
        }

        [Test]
        public void IdentityService_RemoveRoleFromUserAsync_NullUserThrowsException()
        {
            // Arrange
            IdentityUser user = null;
            var role = A.Dummy<string>();

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.RemoveRoleFromUserAsync(user, role));
        }

        [Test]
        public void IdentityService_RemoveRoleFromUserAsync_NullRoleThrowsException()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            var role = string.Empty;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.RemoveRoleFromUserAsync(user, role));
        }

        [Test]
        public async Task IdentityService_RemoveRoleFromUserAsync_NoMatchingRole_FailsSilently()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            var role = "TEST_ROLE";
            var roleList = new List<string>()
            {
                "STANDARD_USER"
            };

            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roleList);

            // Act
            await _identityService.RemoveRoleFromUserAsync(user, role);

            // Assert
            A.CallTo(() => _userManager.RemoveFromRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task IdentityServivce_RemoveRoleFromUserAsync_MatchingRole_RemovesRole()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            var role = "TEST_ROLE";
            var roleList = new List<string>()
            {
                "STANDARD_USER",
                role
            };
            var removed = false;

            A.CallTo(() => _roleManager.RoleExistsAsync(A<string>.Ignored))
                .Returns(true);

            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roleList);

            A.CallTo(() => _userManager.RemoveFromRoleAsync(A<IdentityUser>.Ignored, A<string>.Ignored))
                .Invokes(() => removed = true)
                .Returns(IdentityResult.Success);

            // Act
            await _identityService.RemoveRoleFromUserAsync(user, role);

            // Assert
            Assert.IsTrue(removed);
        }

        [Test]
        public void IdentityService_GetUserRoles_NullUserObject_ThrowsException()
        {
            // Arrange
            IdentityUser user = null;

            // Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _identityService.GetUserRolesAsync(user));
        }

        [Test]
        public async Task IdentityService_GetUserRoles_ReturnsListOfString()
        {
            // Arrange
            const int numRoles = 3;
            var user = A.Fake<IdentityUser>();
            var roles = A.CollectionOfDummy<string>(3).ToList();

            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);

            // Act
            var result = await _identityService.GetUserRolesAsync(user);

            // Assert
            Assert.AreEqual(numRoles, result.Count());
        }

        [Test]
        public async Task IdentityService_GetUserRoles_NoRolesReturnsEmptyList()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            var roles = new List<string>();

            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);

            // Act
            var result = await _identityService.GetUserRolesAsync(user);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task IdentityService_GetUserRoles_NullRolesReturnsEmptyList()
        {
            // Arrange
            var user = A.Fake<IdentityUser>();
            List<string> roles = null;

            A.CallTo(() => _userManager.GetRolesAsync(A<IdentityUser>.Ignored))
                .Returns(roles);

            // Act
            var result = await _identityService.GetUserRolesAsync(user);

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
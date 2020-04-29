using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public IdentityService(UserManager<IdentityUser> user, SignInManager<IdentityUser> signIn, RoleManager<IdentityRole> role, ITokenService token)
        {
            _userManager = user;
            _signInManager = signIn;
            _roleManager = role;
            _tokenService = token;
        }
        public async Task<IdentityUser> CreateUserAsync(string email, string password)
        {
            // Validate the email and password strings
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            // Verify a user does not exist with the same email
            var exists = await GetUserByEmailAsync(email) != null;
            if (exists)
            {
                throw new ResourceConflictException();
            }

            // Create the new user object
            var user = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            // Create the object in the database
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new ActionFailedException(result.Errors.Select(x => x.Description));
            }

            // Return the new user object
            return await GetUserByEmailAsync(email);
        }

        public async Task DeleteUserAsync(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            // Find the user object, throw an exception if none exists
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                throw new ResourceNotFoundException();
            }

            // Delete the user object, throw an exception if the action fails
            var result = await _userManager.DeleteAsync(user);
            if (result != IdentityResult.Success)
            {
                throw new ActionFailedException();
            }
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
        {
            IQueryable<IdentityUser> users = null;
            await Task.Run(() => {
                users = _userManager.Users;
            });
            
            return users;
        }

        public async Task<IdentityUser> GetUserByIdAsync(string id)
        {
            // Validate the ID string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            // Retrieve and return the user object
            var user = await _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            // Validate the email string
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            // Retrieve and return the user object
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task UpdateUserAsync(IdentityUser update)
        {
            // Validate the user object
            if (update == null)
            {
                throw new ArgumentNullException();
            }

            // Update the user object
            var result = await _userManager.UpdateAsync(update);

            // Verify the success of the action
            if (result != IdentityResult.Success)
            {
                throw new ActionFailedException();
            }
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // Validate the email and password strings
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            // Retrieve the user object, verifying it exists
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new ResourceNotFoundException();
            }

            // Attempt to login
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            // Validate the login success
            if (result != SignInResult.Success)
            {
                throw new ArgumentException();
            }

            var token = await _tokenService.MakeToken(user);
            return token;
        }

        public async Task<IdentityUser> GetUserFromToken(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                throw new ArgumentNullException();
            }

            var userId = _tokenService.GetUserId(jwt);

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("invalid token");
            }

            var user = await GetUserByIdAsync(userId);
            return user;
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            // Validate the role name
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException();
            }

            // Use the role manager to check if a role exists
            // with the given name - return the result
            var exists = await _roleManager.RoleExistsAsync(role);
            return exists;
        }

        public async Task<IdentityRole> GetRoleAsync(string name)
        {
            // Validate the role name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            // Check if the role exists - if it doesn't, return null
            var exists = await RoleExistsAsync(name);
            if (!exists)
            {
                return null;
            }

            // Retrieve and return the role
            var role = await _roleManager.FindByNameAsync(name);
            return role;
        }

        public async Task CreateRoleAsync(string name)
        {
            // Validate the role name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            // If the role already exists, do nothing
            var exists = await RoleExistsAsync(name);
            if (exists)
            {
                return;
            }

            // Create a new role with the given name
            var role = new IdentityRole()
            {
                Name = name
            };

            // Add the role to the store
            await _roleManager.CreateAsync(role);
        }

        public async Task DeleteRoleAsync(string name)
        {
            // Validate the role name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            // Check if the role exists - if it doesn't exist,
            // don't do anything
            var exists = await RoleExistsAsync(name);
            if (!exists)
            {
                return;
            }

            // Retrieve the role from the store
            var role = await GetRoleAsync(name);

            // Use the role manager to delete the role
            await _roleManager.DeleteAsync(role);
        }

        public async Task AddRoleToUserAsync(IdentityUser user, string name)
        {
            // Validate the user object
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            // Validate the role name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            // Check if the role exists - If it doesn't exist,
            // throw an exception
            var exists = await RoleExistsAsync(name);
            if (!exists)
            {
                throw new ArgumentException("role does not exist");
            }

            // Add the user to the role
            await _userManager.AddToRoleAsync(user, name);
        }

        public async Task RemoveRoleFromUserAsync(IdentityUser user, string name)
        {
            // Validate the user object
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // Validate the role name
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            // Check if the role exists - if it doesn't exist, do nothing
            var exists = await RoleExistsAsync(name);
            if (!exists)
            {
                return;
            }

            // Retrieve the user's current roles
            var currentRoles = await GetUserRolesAsync(user);

            // Remove the user from the role, if applicable
            if (currentRoles.Contains(name))
            {
                await _userManager.RemoveFromRoleAsync(user, name);
            }
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(IdentityUser user)
        {
            // Validate the user object
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            // Retrieve the list of roles
            var list = await _userManager.GetRolesAsync(user);

            // Validate the list object
            if (list == null)
            {
                list = new List<string>();
            }

            // Return the list
            return list;
        }
    }
}
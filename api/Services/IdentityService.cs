using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Contracts;
using API.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    /// <summary>
    /// Provides services for identity and user management.
    /// </summary>
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

        #region Users

        /// <summary>
        /// Creates a new user object in the identity store.
        /// </summary>
        /// <param name="request">The registration request containing the user name, password, and list of roles.</param>
        /// <returns>Returns a new IdentityUser object for the given registration data.</returns>
        public async Task<IdentityUser> CreateUserAsync(IdentityUser newUser, string password)
        {
            // Validate the email and password strings
            if (string.IsNullOrEmpty(newUser.Email))
            {
                throw new ArgumentNullException("email");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            // Verify a user does not exist with the same email
            var exists = await GetUserByEmailAsync(newUser.Email) != null;
            if (exists)
            {
                throw new ResourceConflictException();
            }

            // Create the new user object
            // var user = new IdentityUser
            // {
            //     Email = request.Email,
            //     UserName = request.Email
            // };

            // Create the user object in the database
            var result = await _userManager.CreateAsync(newUser, password);
            if (!result.Succeeded)
            {
                throw new ActionFailedException(result.Errors.Select(x => x.Description));
            }

            // Add the user to the "User" role
            await AddRoleToUserAsync(newUser, Contracts.Roles.User);

            // Return the new user object
            return await GetUserByEmailAsync(newUser.Email);
        }

        /// <summary>
        /// Deletes a user object from the identity store.
        /// </summary>
        /// <param name="id">The ID for the user to delete.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a list of all user objects in the identity store.
        /// </summary>
        /// <returns>An IEnumerable of IdentityUser objects</returns>
        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
        {
            IQueryable<IdentityUser> users = null;
            await Task.Run(() => {
                users = _userManager.Users;
            });
            
            return users;
        }

        /// <summary>
        /// Retrieves the IdentityUser with the matching ID.
        /// </summary>
        /// <param name="id">The ID by which to search for the user object.</param>
        /// <returns>An IdentityUser object or null</returns>
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

        /// <summary>
        /// Retrieves the user object from the identity store with the matching email address.
        /// </summary>
        /// <param name="email">The email for which to search.</param>
        /// <returns>An IdentityUser object or null.</returns>
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

        /// <summary>
        /// Updates an IdentityUser object in the identity store.
        /// </summary>
        /// <param name="update">A UserUpdateRequest object containing changed information</param>
        /// <returns></returns>
        public async Task UpdateUserAsync(IdentityUser update)
        {
            // Validate the user object
            if (update == null)
            {
                throw new ArgumentNullException();
            }

            // Verify the user exists
            var exists = await UserExistsWithIdAsync(update.Id);
            if (!exists)
            {
                throw new ResourceNotFoundException();
            }
            // var user = await GetUserByIdAsync(update.Id);
            // if (user == null)
            // {
            //     throw new ResourceNotFoundException();
            // }

            // Map the update object to the user, updating any changed
            // information
            // user = user.Update(update);

            // Update the user object
            var result = await _userManager.UpdateAsync(update);

            // Verify the success of the action
            if (result != IdentityResult.Success)
            {
                throw new ActionFailedException();
            }

            // // Update the user's roles
            // if (update.Roles != null)
            // {
            //     await UpdateUserRoles(user, update.Roles);
            // }
        }

        /// <summary>
        /// Verifies a UserLoginRequest and returns a JWT when successful.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>A string containing a JWT</returns>
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

        /// <summary>
        /// Sets the user's password
        /// </summary>
        /// <param name="user">The existing IdentityUser for which to set the password</param>
        /// <param name="password">The plain-text password - will be hashed and stored</param>
        /// <returns></returns>
        public async Task SetUserPasswordAsync(IdentityUser user, string password)
        {
            var validator = new PasswordValidator<IdentityUser>();
            var isValid = await validator.ValidateAsync(_userManager, null, password);

            if (!isValid.Succeeded) throw new ArgumentException("Invalid password");

            // Validate user object
            if (user == null)
            {
                throw new ArgumentNullException("User cannot be null");
            }

            // Validate password string
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("Password cannot be empty");
            }

            // Check if the user has a password set
            var hasPassword = await _userManager.HasPasswordAsync(user);

            // If the user has a password currently set, remove it
            if (hasPassword)
            {
                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    // If the password could not be removed, throw an exception
                    throw new ActionFailedException();
                }
            }

            // Set the new password
            IdentityResult setResult = null;
            try
            {
                setResult = await _userManager.AddPasswordAsync(user, password);
            }
            catch (Exception e) 
            {
                throw new ActionFailedException();
            }

            if (!setResult.Succeeded)
            {
                // If the new password could not be set, throw an exception
                throw new ActionFailedException();
            }
        }

        /// <summary>
        /// Retrieves an IdentityUser object from a given JWT
        /// </summary>
        /// <param name="jwt">A string representation of a JWT</param>
        /// <returns>An IdentityUser object or null</returns>
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

        /// <summary>
        /// Confirms whether or not a JWT represents a valid, 
        /// unexpired token
        /// </summary>
        /// <param name="jwt">A string containing a JWT</param>
        /// <returns>True if the token is valid, false if not</returns>
        public async Task<bool> ValidateTokenAsync(string jwt)
        {
            // Validate the jwt string
            if (string.IsNullOrEmpty(jwt))
            {
                throw new ArgumentNullException();
            }

            bool isValid = false;

            // Run the check asynchronously
            await Task.Run(() => {
                isValid = _tokenService.IsValid(jwt);
            });

            return isValid;
        }

        public async Task SetUserRolesAsync(IdentityUser user, IEnumerable<string> roles)
        {
            // Validate the user object
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            // Validate the roles object
            if (roles == null)
            {
                throw new ArgumentNullException();
            }

            // Validate the roles to be set
            foreach (var role in roles)
            {
                if (!(await _roleManager.RoleExistsAsync(role)))
                {
                    throw new ArgumentOutOfRangeException($"role {role} does not exist");
                }
            }

            // Get the list of current roles for the user
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Loop through the user's current roles
            foreach (var role in currentRoles)
            {
                // If the role is not included in the set roles, remove it from the user
                if (!roles.Contains(role))
                {
                    var removed = await _userManager.RemoveFromRoleAsync(user, role);
                    if (!removed.Succeeded)
                    {
                        throw new ActionFailedException();
                    }
                }
            }

            // Update the list of current roles
            currentRoles = await _userManager.GetRolesAsync(user);

            // Loop through the set of roles
            foreach (var role in roles)
            {
                // If the user is not currently assigned to the role, add them to the role
                if (!currentRoles.Contains(role))
                {
                    var added = await _userManager.AddToRoleAsync(user, role);
                    if (!added.Succeeded)
                    {
                        throw new ActionFailedException();
                    }
                }
            }
        }

        /// <summary>
        /// Verifies if a user exists with a given ID
        /// </summary>
        /// <param name="id">The user ID to search for</param>
        /// <returns>True if a user exists with the ID, false if none are found</returns>
        public async Task<bool> UserExistsWithIdAsync(string id)
        {
            // Validate string
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException();
            }

            var user = await GetUserByIdAsync(id);

            return user != null;
        }

        /// <summary>
        /// Verifies if a user exists with a given email address
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <returns>True if a user exists with the email address, false if none are found</returns>
        public async Task<bool> UserExistsWithEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            var user = await GetUserByEmailAsync(email);

            return user != null;
        }

        #endregion

        #region Roles

        /// <summary>
        /// Confirms the existence of a role.
        /// </summary>
        /// <param name="role">The Role name</param>
        /// <returns>True if the role exists, false if not</returns>
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

        /// <summary>
        /// Retrieves an IdentityRole object from the identity store
        /// </summary>
        /// <param name="name">The name of the role to retrieve</param>
        /// <returns>An IdentityRole object or null</returns>
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

        /// <summary>
        /// Creates a new IdentityRole object in the identity store
        /// </summary>
        /// <param name="name">The name of the role to create</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes an IdentityRole object from the identity store
        /// </summary>
        /// <param name="name">The name of the role to delete</param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates a user's roles to match a list of role names
        /// </summary>
        /// <param name="user">The IdentityUser to update</param>
        /// <param name="roles">A list of role names</param>
        /// <returns></returns>
        private async Task UpdateUserRoles(IdentityUser user, IEnumerable<string> roles)
        {
            // Get the user's list of currently assigned roles
            var currentRoles = await GetUserRolesAsync(user);

            // Add any additional roles to the user
            foreach (var role in roles)
            {
                if (!currentRoles.Contains(role))
                {
                    await AddRoleToUserAsync(user, role);
                }
            }

            // Remove any roles that have been unassigned
            foreach (var role in currentRoles)
            {
                if (!roles.Contains(role))
                {
                    await RemoveRoleFromUserAsync(user, role);
                }
            }
        }

        /// <summary>
        /// Adds a user to a list of roles
        /// </summary>
        /// <param name="user">The IdentityUser to update</param>
        /// <param name="roles">A list of role names to attach to the user</param>
        /// <returns></returns>
        private async Task AddRolesToUserAsync(IdentityUser user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var exists = await RoleExistsAsync(role);
                if (!exists)
                {
                    await CreateRoleAsync(role);
                }

                await AddRoleToUserAsync(user, role);
            }
        }

        /// <summary>
        /// Adds a single role to a user
        /// </summary>
        /// <param name="user">The IdentityUser to update</param>
        /// <param name="name">The name of the role to attach to the user</param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes a single role from a user
        /// </summary>
        /// <param name="user">The IdentityUser to update</param>
        /// <param name="name">The name of the role to remove</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves the roles currently assigned to a user
        /// </summary>
        /// <param name="user">The IdentityUser for whom to retrieve roles</param>
        /// <returns></returns>
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

        #endregion
    }
}
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
                Email = email
            };

            // Create the object in the database
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new ActionFailedException();
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
    }
}
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private UserManager<IdentityUser> _userManager;

        public TokenService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string GetUserId(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                throw new ArgumentNullException();
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var userId = token.Claims.FirstOrDefault(x => x.Type == Contracts.Claims.UserId);
            return userId.Value;
        }

        public async Task<string> MakeToken(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            var key = GetTokenKey();
            var tokenDescriptor = await GetDescriptor(user, key);
            var token = CreateToken(tokenDescriptor);

            return token;
        }

        private byte[] GetTokenKey()
        {
            var secret = System.Environment.GetEnvironmentVariable("TOKEN_SECRET");
            var key = Encoding.ASCII.GetBytes(secret);
            return key;
        }

        private string CreateToken(SecurityTokenDescriptor descriptor)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descriptor);
            var tokenString = handler.WriteToken(token);
            return tokenString;
        }

        private async Task<List<Claim>> GetUserClaims(IdentityUser user)
        {
            var userClaims = new List<Claim>();
            var userRoles = await _userManager.GetRolesAsync(user);

            userClaims.AddRange(userRoles.Select(role => 
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            ));

            return userClaims;
        }

        private async Task<SecurityTokenDescriptor> GetDescriptor(IdentityUser user, byte[] key)
        {
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.NormalizedEmail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(Contracts.Claims.UserId, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(Contracts.Claims.ExpiresInDays),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256
                )
            };

            var userClaims = await GetUserClaims(user);

            return tokenDescriptor;
        }
    }
}
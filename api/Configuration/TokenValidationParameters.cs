using Microsoft.IdentityModel.Tokens;

namespace API.Configuration
{
    public static class TokenValidationParametersFactory
    {
        public static TokenValidationParameters Get(byte[] key)
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
    }
}
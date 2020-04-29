namespace API.Contracts
{
    public class TokenResponse
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        public TokenResponse(string userId, string token)
        {
            UserId = userId;
            Token = token;
        }
    }
}
namespace API.Services
{
    public interface ITokenService
    {
        string MakeToken();
        bool AuthenticateToken(string token);
        string GetUserId(string token);
    }
}
namespace API.Contracts
{
    public class UpdateUserRequest
    {
        public string Email { get; set; } = null;
        public string Password { get; set; } = null;
        public string Role { get; set; } = null;
    }
}
namespace API.Contracts
{
    public static class Routes
    {
        public const string Controller = "[controller]";
        public static class Logs
        {
            public const string Get = "";
            public const string GetById = "{id}";
            public const string GetForUser = "user/{id}";
            public const string Update = "update/{id}";
            public const string Delete = "delete/{id}";
            public const string Save = "save";
            public const string Upload = "upload";
        }

        public static class Identity
        {
            public const string GetAllUsers = "";
            public const string GetUserById = "{id}";
            public const string GetUserByEmail = "email/{email}";
            public const string CreateUser = "";
            public const string UpdateUser = "{id}";
            public const string DeleteUser = "{id}";
            public const string Login = "login";
        }
    }
}
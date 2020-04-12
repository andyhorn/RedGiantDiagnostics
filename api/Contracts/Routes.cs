namespace API.Contracts
{
    public static class Routes
    {
        public const string Controller = "[controller]";
        public static class Logs
        {
            public const string Get = "";
            public const string GetById = "/{id}";
            public const string GetForUser = "/user/{id}";
            public const string Update = "/{id}";
            public const string Delete = "/{id}";
            public const string Save = "/save";
            public const string Upload = "/";
        }
    }
}
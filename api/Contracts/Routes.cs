namespace API.Contracts
{
    public static class Routes
    {
        public const string ControllerV1 = "api/v1/{controller}";
        public const string ControllerV2 = "api/v2/{controller}";
        public static class Administrator
        {
            public static class Users
            {
                private const string Base = "users";
                public const string Register = Base;
                public const string GetAll = Base;
                public const string Update = Base + "/{id}";
                public const string Roles = Base + "/{id}/roles";
                public const string Delete = Base + "/{id}";
                public const string GetById = Base + "/{id}";
                public const string GetByEmail = Base + "/email/{email}";
                public const string SetPassword = Base + "/{id}/password";
            }

            public static class Logs
            {
                private const string Base = "logs";
                public const string Update = Base + "/{id}";
                public const string GetAll = Base;
                public const string Delete = Base + "/{id}";
                public const string Analytics = Base + "/analytics";
                public const string AnalysisTypes = Base + "/analytics/types";
            }
        }
        public static class Logs
        {
            public static class V2
            {
                public const string GetById = "{id}";
                public const string Update = "{id}";
                public const string Delete = "{id}";
                public const string Save = "";
                public const string Upload = "upload";
                public const string GetLogsForCurrentUser = "";
            }
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
            public static class V2
            {
                public const string Get = "";
                public const string Update = "{id}";
                public const string Login = "Login";
                public const string ChangePassword = "{id}/Password";
            }
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
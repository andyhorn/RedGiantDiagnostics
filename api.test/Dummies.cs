using API.Contracts;
using API.Contracts.Requests;
using API.Contracts.Requests.Admin;
using FakeItEasy;

namespace api.test
{
    public class DummyUserUpdateRequestFactory : DummyFactory<UserUpdateRequest>
    {
        protected override UserUpdateRequest Create()
        {
            return new UserUpdateRequest
            {
                FirstName = A.Dummy<string>(),
                LastName = A.Dummy<string>(),
                Email = A.Dummy<string>()
            };
        }
    }

    public class DummyUserRegistrationRequestFactory : DummyFactory<AdminUserRegistrationRequest>
    {
        protected override AdminUserRegistrationRequest Create()
        {
            return new AdminUserRegistrationRequest
            {
                Email = A.Dummy<string>(),
                FirstName = A.Dummy<string>(),
                LastName = A.Dummy<string>(),
                Password = A.Dummy<string>(),
                ConfirmPassword = A.Dummy<string>()
            };
        }
    }

    public class DummyStringFactory : DummyFactory<string>
    {
        protected override string Create() => "DummyString";
    }
}
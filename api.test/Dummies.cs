using API.Contracts;
using FakeItEasy;

namespace api.test
{
    public class DummyUserUpdateRequestFactory : DummyFactory<UserUpdateRequest>
    {
        protected override UserUpdateRequest Create()
        {
            return new UserUpdateRequest
            {
                Id = A.Dummy<string>(),
                Email = A.Dummy<string>()
            };
        }
    }

    public class DummyUserRegistrationRequestFactory : DummyFactory<UserRegistrationRequest>
    {
        protected override UserRegistrationRequest Create()
        {
            return new UserRegistrationRequest
            {
                Email = A.Dummy<string>(),
                Password = A.Dummy<string>()
            };
        }
    }
}
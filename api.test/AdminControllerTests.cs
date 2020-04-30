using API.Controllers.V2;
using API.Services;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class AdminControllerTests
    {
        private IIdentityService _identityService;
        private ILogsService _logsService;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            _identityService = A.Fake<IIdentityService>();
            _logsService = A.Fake<ILogsService>();
            _controller = new AdminController(_identityService, _logsService);
        }
    }
}
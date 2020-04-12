using NUnit.Framework;
using FakeItEasy;

namespace api.tests
{
    public class LogsControllerTests
    {
        private LogsController _logsController = new LogsController();
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
using API.Entities;
using API.Factories;
using NUnit.Framework;

namespace api.test
{
    public class ServerStatusFactoryTests
    {
        private IServerStatusFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new ServerStatusFactory();
        }

        [Test]
        public void ServerStatusFactory_New_ReturnsNewServerStatusObject()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(ServerStatus), result);
        }

        [Test]
        public void ServerStatusFactory_Parse_NullDataReturnsNullObject()
        {
            // Arrange

            // Act
            var result = _factory.Parse(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ServerStatusFactory_Parse_HandlesOneColumn()
        {
            // Arrange
            const string data = "datadatadata";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(data, result.Name);
        }

        [Test]
        public void ServerStatusFactory_Parse_HandlesTwoColumns()
        {
            // Arrange
            const string name = "Name";
            const string port = "1234";
            int portNum = int.Parse(port);
            string data = $"{name} {port}";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(portNum, result.Port);
        }

        [Test]
        public void ServerStatusFactory_Parse_HandlesInvalidPortInteger()
        {
            // Arrange
            string data = "Name NotAnInteger";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.Port);
        }

        [Test]
        public void ServerStatusFactory_Parse_HandlesThreeColumns()
        {
            // Arrange
            string data = "Test Test Test";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ServerStatusFactory_Parse_CorrectlyMatchesYesRunning()
        {
            // Arrange
            const string data = "Test Test Yes";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(result.IsRunning);
        }

        [Test]
        public void ServerStatusFactory_Parse_CorrectlyMatchesNotRunning()
        {
            // Arrange
            const string data = "Test Test NotAYes";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsFalse(result.IsRunning);
        }

        [Test]
        public void ServerStatusFactory_Parse_HandlesFourColumns()
        {
            // Arrange
            const string data = "Test Test Test Test";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ServerStatusFactory_Parse_ParsesRestartInteger()
        {
            // Arrange
            const int numRestarts = 5;
            string data = $"Test Test Test {numRestarts}";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(numRestarts, result.Restarts);
        }

        [Test]
        public void ServerStatusFactory_Parse_HandlesInvalidRestartValue()
        {
            // Arrange 
            const string data = "Test Test Test NotAnInteger";

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.Restarts);
        }
    }
}
using API.Entities;
using API.Factories;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class LogFactoryTests
    {
        private ILogFactory _factory;
        private ILogParserFactory _logParserFactory;

        [SetUp]
        public void Setup()
        {
            _logParserFactory = A.Fake<ILogParserFactory>();
            _factory = new LogFactory(_logParserFactory);
        }

        [Test]
        public void LogFactory_New_ReturnsNewLogFileObject()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(LogFile), result);
        }

        [Test]
        public void LogFactory_Parse_NullStringReturnsNull()
        {
            // Arrange
            string nullString = null;

            // Act
            var result = _factory.Parse(nullString);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void LogFactory_Parse_EmptyStringReturnsNull()
        {
            // Arrange
            string emptyString = string.Empty;

            // Act
            var result = _factory.Parse(emptyString);

            // Assert
            Assert.IsNull(result);
        }
    }
}
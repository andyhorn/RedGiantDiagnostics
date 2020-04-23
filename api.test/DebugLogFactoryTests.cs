using System.Linq;
using API.Entities;
using API.Factories;
using NUnit.Framework;

namespace api.test
{
    public class DebugLogFactoryTests
    {
        private IDebugLogFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new DebugLogFactory();
        }

        [Test]
        public void DebugLogFactory_New_ReturnsNewDebugLog()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(DebugLog), result);
        }

        [Test]
        public void DebugLogFactory_Parse_EmptyStringArrayReturnsNull()
        {
            // Arrange
            var emptyStringArray = new string[0];

            // Act
            var result = _factory.Parse(emptyStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void DebugLogFactory_Parse_NullStringArrayReturnsNull()
        {
            // Arrange
            string[] nullStringArray = null;

            // Act
            var result = _factory.Parse(nullStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void DebugLogFactory_Parse_ReturnValidFilename()
        {
            // Arrange
            const string filename = "Log_filename";
            string[] filenameSection = new string[]
            {
                $"debug log file contents ({filename})"
            };

            // Act
            var result = _factory.Parse(filenameSection);

            // Assert
            Assert.IsInstanceOf(typeof(DebugLog), result);
            Assert.AreEqual(filename, result.Filename);
        }

        [Test]
        public void DebugLogFactory_Parse_LogLinesIgnoresOutsideBarriers()
        {
            // Arrange
            const string pass = "PASS";
            string[] logLines = new string[]
            {
                "==========================",
                pass,
                "=========================="
            };

            // Act
            var result = _factory.Parse(logLines);

            // Assert
            Assert.IsInstanceOf(typeof(DebugLog), result);

            var containsPass = result.Lines.ToList().Any(x => x.Contains(pass));
            Assert.IsTrue(containsPass);
        }
    }
}
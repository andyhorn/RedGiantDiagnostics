using System.Linq;
using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class RlmInstanceFactoryTests
    {
        private IUtilities _utilities;
        private IRlmInstanceFactory _factory;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();
            _factory = new RlmInstanceFactory(_utilities);
        }

        [Test]
        public void RlmInstanceFactory_New_ReturnsNewRlmInstanceObject()
        {
            // Arrange
            
            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(RlmInstance), result);
        }

        [Test]
        public void RlmInstanceFactory_Parse_NullInputDataReturnsNull()
        {
            // Arrange
            string[] nullInput = null;

            // Act
            var result = _factory.Parse(nullInput);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void RlmInstanceFactory_Parse_EmptyInputDataReturnsNull()
        {
            // Arrange
            string[] emptyInput = new string[0];

            // Act
            var result = _factory.Parse(emptyInput);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void RlmInstanceFactory_Parse_SavesRlmVersion()
        {
            // Arrange
            string returns = "Version";
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("RLM Version:"),
                A<int>.That.IsEqualTo(2),
                A<string[]>._
            ))
            .Returns(returns);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(returns, result.Version);
        }

        [Test]
        public void RlmInstanceFactory_Parse_SavesCommand()
        {
            // Arrange
            const string command = "Command";
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Command:"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(command);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(command, result.Command);
        }

        [Test]
        public void RlmInstanceFactory_Parse_SavesWorkingDirectory()
        {
            // Arrange
            const string workingDir = "WorkingDirectory";
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Working Directory:"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(workingDir);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(workingDir, result.WorkingDirectory);
        }

        [Test]
        public void RlmInstanceFactory_Parse_PID_InvalidInteger_SavesZero()
        {
            // Arrange
            const string invalid = "NotAnInteger";
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("PID:"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(invalid);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.PID);
        }

        [Test]
        public void RlmInstanceFactory_Parse_PID_SavesValidInteger()
        {
            // Arrange
            const string number = "5";
            int shouldReturn = int.Parse(number);
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("PID:"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(number);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(shouldReturn, result.PID);
        }

        [Test]
        public void RlmInstanceFactory_Parse_Port_SavesValidInteger()
        {
            // Arrange
            const string number = "5";
            int shouldReturn = int.Parse(number);
            var data = A.CollectionOfDummy<string>(5).ToArray();
            
            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Main TCP/IP port:"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(number);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(shouldReturn, result.Port);
        }

        [Test]
        public void RlmInstanceFactory_Parse_WebPort_SavesValidInteger()
        {
            // Arrange
            const string number = "5";
            int shouldReturn = int.Parse(number);
            var data = A.CollectionOfDummy<string>(5).ToArray();
            
            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Web interface"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(number);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(shouldReturn, result.Port);
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetAlternativePorts_HandlesNullLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            
            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Alternate"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.AlternativePorts);
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetAlternativePorts_HandlesEmptyLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            
            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Alternate"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(string.Empty);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.AlternativePorts);
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetAlternativePorts_InvalidIntegersNotAddedToList()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string returnValue = "NotANumber";

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Alternate"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(returnValue);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.AlternativePorts.Count());
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetAlternativePorts_ValidIntegersSavedToList()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string integers = "1 2 3 4 5";

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("Alternate"),
                A<string>.That.IsEqualTo(":"),
                A<string[]>.Ignored
            ))
            .Returns(integers);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(5, result.AlternativePorts.Count());
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetIsvServers_NullLineValueReturnsEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("ISV servers:"),
                A<string>.Ignored,
                A<string[]>.Ignored
            ))
            .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.IsvServers);
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetIsvServers_EmptyLineValueReturnsEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("ISV servers:"),
                A<string>.Ignored,
                A<string[]>.Ignored
            ))
            .Returns(string.Empty);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.IsvServers);
        }

        [Test]
        public void RlmInstanceFactory_Parse_GetIsvServers_ReturnsValidList()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string serverOne = "ServerOne";
            const string serverTwo = "ServerTwo";
            string serverList = $"{serverOne} {serverTwo}";

            A.CallTo(() => _utilities.GetAfter(
                A<string>.That.IsEqualTo("ISV servers:"),
                A<string>.Ignored,
                A<string[]>.Ignored
            ))
            .Returns(serverList);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.Contains(serverOne, result.IsvServers.ToList());
            Assert.Contains(serverTwo, result.IsvServers.ToList());
        }
    }
}
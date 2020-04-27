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

        
    }
}
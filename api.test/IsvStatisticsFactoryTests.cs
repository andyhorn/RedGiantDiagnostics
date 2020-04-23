using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class IsvStatisticsFactoryTests
    {
        private IIsvStatisticsFactory _factory;
        private API.Helpers.IUtilities _utilities;
        private ILicensePoolFactory _licensePoolFactory;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();
            _licensePoolFactory = A.Fake<ILicensePoolFactory>();

            _factory = new IsvStatisticsFactory(_utilities, _licensePoolFactory);
        }

        [Test]
        public void IsvStatisticsFactory_New_ReturnsIsvStatisticsObject()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(IsvStatistics), result);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_NullStringArrayReturnsNull()
        {
            // Arrange
            string[] nullStringArray = null;

            // Act
            var result = _factory.Parse(nullStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_EmptyStringArrayReturnsNull()
        {
            // Arrange
            string[] emptyStringArray = new string[0];

            // Act
            var result = _factory.Parse(emptyStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_ParsesServerName()
        {
            // Arrange
            const string serverName = "SERVER_NAME";
            string[] data = new string[]
            {
                $"ISV {serverName} status on such-and-such machine"
            };

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(DebugLog), result);
            Assert.AreEqual(serverName, result.ServerName);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_ParsesLicensePools()
        {
            // TODO: Create IsvStatisticsFactory license pool test once
            // license pool factory is completed
        }
    }
}
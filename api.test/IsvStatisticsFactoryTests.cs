using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Factories;
using API.Helpers;
using API.Models;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class IsvStatisticsFactoryTests
    {
        private IIsvStatisticsFactory _factory;
        private IUtilities _utilities;
        private IStatisticsParser _statisticsParsers;
        private ILicensePoolFactory _licensePoolFactory;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();
            _statisticsParsers = A.Fake<IStatisticsParser>();
            _licensePoolFactory = A.Fake<ILicensePoolFactory>();

            _factory = new IsvStatisticsFactory(_utilities, _licensePoolFactory, _statisticsParsers);
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
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(serverName);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(IsvStatistics), result);
            Assert.AreEqual(serverName, result.ServerName);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_NoLicensePoolData_ReturnsEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            string[] emptyList = new string[0];
            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(emptyList);
            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.LicensePools);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_NoLicensePoolSections_ReturnsEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            var emptyCollection = new List<List<string>>();
            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(data);
            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .Returns(emptyCollection);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.LicensePools);
        }

        [Test]
        public void IsvStatisticsFactory_Parse_NullLicensePoolNotAddedToList()
        {
            // Arrange
            var dummyData = A.CollectionOfDummy<string>(5).ToArray();
            var dummyDummyData = new List<List<string>>();
            dummyDummyData.Add(A.CollectionOfDummy<string>(5).ToList());

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(dummyData);
            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .Returns(dummyDummyData);
            A.CallTo(() => _licensePoolFactory.Parse(A<string[]>.Ignored))
                .Returns(null);
            
            // Act
            var result = _factory.Parse(dummyData);

            // Assert
            Assert.IsEmpty(result.LicensePools);
        }
    }
}
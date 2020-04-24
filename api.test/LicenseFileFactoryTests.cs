using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class LicenseFileFactoryTests
    {
        private ILicenseFileFactory _factory;
        private IProductLicenseFactory _productLicenseFactory;
        private IUtilities _utilities;

        [SetUp]
        public void Setup()
        {
            _productLicenseFactory = A.Fake<IProductLicenseFactory>();
            _utilities = A.Fake<IUtilities>();
            _factory = new LicenseFileFactory(_utilities, _productLicenseFactory);
        }

        [Test]
        public void LicenseFileFactory_New_ReturnsNewLicenseFile()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(LicenseFile), result);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseHostMac_ReturnsProperMacAddress()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string realMac = "12:34:56:78:90:AB";
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(realMac);
            A.CallTo(() => _utilities.MakeMac(A<string>.Ignored))
                .Returns(realMac);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(realMac, result.HostMac);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseHostMac_NullHostLine_ReturnsEmptyString()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.HostMac);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseHostMac_RemovesEtherSection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string dirtyMac = "ether=1234567890ab";
            const string cleanMac = "1234567890ab";
            bool isClean = false;
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(dirtyMac);
            A.CallTo(() => _utilities.MakeMac(A<string>.That.Matches(x => x == cleanMac)))
                .Invokes(() => isClean = true)
                .Returns(A.Dummy<string>());

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(isClean);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseIsvPort_NoIsvLineReturnsEmptyString()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.IsvPort);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseIsvPort_RemovesPortHeader()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string isvLineWithPortHeader = "port=5055";
            const string isvLineWithNoPortHeader = "5055";
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(isvLineWithPortHeader);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(isvLineWithNoPortHeader, result.IsvPort);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseProducts_NullSubsectionsReturnsEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.ProductLicenses);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseProducts_EmptySubsectionsReturnsEmptyCollection()
        {
            // Arrange
            var emptyCollection = new List<List<string>>();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .Returns(emptyCollection);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.ProductLicenses);
        }

        [Test]
        public void LicenseFileFactory_Parse_GetLicenseProducts_NullProductLicensesNotAddedToList()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            var dummyCollection = A.CollectionOfDummy<string>(5).ToList();
            var list = new List<List<string>>();
            list.Add(dummyCollection);
            list.Add(dummyCollection);
            var returnList = new List<ProductLicense>();
            returnList.Add(null);
            returnList.Add(A.Fake<ProductLicense>());

            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .Returns(list);

            A.CallTo(() => _productLicenseFactory.Parse(A<string[]>.Ignored))
                .ReturnsNextFromSequence(returnList.ToArray());

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(1, result.ProductLicenses.Count());
        }
    }
}
using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class ProductLicenseFactoryTests
    {
        private IUtilities _utilities;
        private ProductLicenseFactory _factory;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();   
            _factory = new ProductLicenseFactory(_utilities);
        }

        [Test]
        public void ProductLicenseFactory_New_ReturnsProductLicenseObject()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(ProductLicense), result);
        }
    }
}
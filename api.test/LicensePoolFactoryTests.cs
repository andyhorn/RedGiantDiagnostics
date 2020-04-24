using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class LicensePoolFactoryTests
    {
        private ILicensePoolFactory _factory;
        private IUtilities _utilities;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();
            _factory = new LicensePoolFactory(_utilities);
        }

        [Test]
        public void LicensePoolFactory_New_ReturnsNewLicensePoolObject()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(LicensePool), result);
        }

        [Test]
        public void LicensePoolFactory_Parse_NullPoolUsageLinesReturnsEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.CheckedOutTo);
        }

        [Test]
        public void LicensePoolFactory_Parse_EmptyPoolUsageLinesReturnsEmptyCollection()
        {
            // Arrange
            var emptyCollection = new List<string>().ToArray();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(emptyCollection);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.CheckedOutTo);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetCheckedOutToList_EmptyCheckoutLinesNotAddedToCollection()
        {
            // Arrange
            var emptyCollection = new string[0];
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(emptyCollection);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.CheckedOutTo);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetCheckout_OnlyFirstColumn()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string user = "Test";
            var misformedCheckoutLine = $"{user}";
            var list = new List<string>();
            list.Add(misformedCheckoutLine);

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(list.ToArray());

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(1, result.CheckedOutTo.Count());
            Assert.IsTrue(result.CheckedOutTo.ToList()[0].Contains(user));
        }

        [Test]
        public void LicensePoolFactory_Parse_GetCheckout_OnlyFirstTwoColumns()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();

            const string valueOne = "TestOne";
            const string valueTwo = "TestTwo";
            var misformedCheckoutLine = $"{valueOne}:{valueTwo}";

            var list = new List<string>();
            list.Add(misformedCheckoutLine);

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(list.ToArray());

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(1, result.CheckedOutTo.Count());
            Assert.IsTrue(result.CheckedOutTo.ToList()[0].Contains(valueOne));
            Assert.IsTrue(result.CheckedOutTo.ToList()[0].Contains(valueTwo));
        }

        [Test]
        public void LicensePoolFactory_Parse_GetCheckout_ReturnsAllValues()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();

            const string valueOne = "TestOne";
            const string valueTwo = "TestTwo";
            const string valueThree = "TestThree";
            var checkoutLine = $"{valueOne}:{valueTwo}:x:x:x:x:x:x:{valueThree}";

            var list = new List<string>();
            list.Add(checkoutLine);

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Returns(list.ToArray());

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(1, result.CheckedOutTo.Count());
            Assert.IsTrue(result.CheckedOutTo.ToList()[0].Contains(valueOne));
            Assert.IsTrue(result.CheckedOutTo.ToList()[0].Contains(valueTwo));
            Assert.IsTrue(result.CheckedOutTo.ToList()[0].Contains(valueThree));
        }
    }
}
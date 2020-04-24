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
        public void LicensePoolFactory_Parse_NullDataArray_ReturnsNull()
        {
            // Arrange
            string[] nullDataArray = null;

            // Act
            var result = _factory.Parse(nullDataArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void LicensePoolFactory_Parse_EmptyDataArray_ReturnsNull()
        {
            // Arrange
            string[] emptyDataArray = new string[0];

            // Act
            var result = _factory.Parse(emptyDataArray);

            // Assert
            Assert.IsNull(result);
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

        [Test]
        public void LicensePoolFactory_Parse_GetInUseSeats_NullLineValueReturnsZero()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.InUse);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetInUseSeats_EmptyLineValueReturnsZero()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(string.Empty);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.InUse);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetInUseSeats_NonIntegerValueReturnsZero()
        {
            // Arrange
            const string notAnInt = "helloworld!";
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(notAnInt);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.InUse);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetInUseSeats_NoInUseHeaderReturnsValue()
        {
            // Arrange
            const string returnValue = "100";
            int returnInt = int.Parse(returnValue);
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(returnValue);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(returnInt, result.InUse);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetInUseSeats_ReturnsValue()
        {
            // Arrange
            const int returnInt = 100;
            string returnString = $"inuse:{returnInt}";
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(returnString);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(returnInt, result.InUse);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetTotalSeats_NullLineValue_ReturnsZero()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.TotalSeats);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetTotalSeats_EmptyLineValue_ReturnsZero()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(string.Empty);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.TotalSeats);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetTotalSeats_NonIntegerString_ReturnsZero()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const string nonInt = "NotAnInteger";
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(nonInt);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(0, result.TotalSeats);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetTotalSeats_NoSoftHeader_ReturnsValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const int returnInt = 5;
            string returnString = returnInt.ToString();
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(returnString);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(returnInt, result.TotalSeats);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetTotalSeats_SoftHeader_ReturnsValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            const int returnInt = 5;
            string returnString = $"Soft:{returnInt}";
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(returnString);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(returnInt, result.TotalSeats);
        }

        [Test]
        public void LicensePoolFactory_Parse_GetProductName_NullLineValue_ReturnsEmptyString()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            string returnString = null;
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .Returns(returnString);
            
            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.Product);
        }
    }
}
using System;
using System.Linq;
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

        [Test]
        public void ProductLicenseFactory_Parse_NullDataReturnsNull()
        {
            // Arrange
            const string[] nullData = null;

            // Act
            var result = _factory.Parse(nullData);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ProductLicenseFactory_Parse_EmptyDataReturnsNull()
        {
            // Arrange
            string[] emptyData = new string[0];

            // Act
            var result = _factory.Parse(emptyData);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductIssueDate_HandlesNullLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;

            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string search, int word, string[] data) => {
                    if (search == "start=" && word == 1)
                    {
                        enteredFunction = true;
                    }

                    return null;
                });

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsNull(result.IssueDate);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductIssueDate_HandlesEmptyLine()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;

            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string search, int word, string[] data) => {
                    if (search == "start=" && word == 1)
                    {
                        enteredFunction = true;
                        return string.Empty;
                    }

                    return null;
                });

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsNull(result.IssueDate);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductIssueDate_ParsesLineWithStartHeader()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            var date = "04/20/2020";
            var enteredFunction = false;
            var removedStartHeader = false;
            
            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string search, int word, string[] data) => {
                    if (search == "start=" && word == 1)
                    {
                        return date;
                    }

                    return null;
                });

            A.CallTo(() => _utilities.GetDateTimeFrom(A<string>.Ignored))
                .ReturnsLazily((string dateString) => {
                    if (dateString.Contains(date))
                    {
                        enteredFunction = true;

                        if (!dateString.Contains("start="))
                        {
                            removedStartHeader = true;
                        }
                    }

                    return null;
                });

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsTrue(removedStartHeader);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductExpirationDate_HandlesNullLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            var enteredFunction = false;

            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string search, int word, string[] data) => {
                    if (search == "LICENSE" && word == 4)
                    {
                        enteredFunction = true;
                    }

                    return null;
                });

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsNull(result.ExpirationDate);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductExpirationDate_HandlesEmptyLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            var enteredFunction = false;

            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string search, int word, string[] data) => {
                    if (search == "LICENSE" && word == 4)
                    {
                        enteredFunction = true;
                        return string.Empty;
                    }

                    return null;
                });

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsNull(result.ExpirationDate);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductExpirationDate_SavesValidDate()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            var enteredFunction = false;
            const string date = "04/20/2020";

            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string search, int word, string[] data) => {
                    if (search == "LICENSE" && word == 4)
                    {
                        return date;
                    }

                    return null;
                });

            A.CallTo(() => _utilities.GetDateTimeFrom(A<string>.Ignored))
                .ReturnsLazily((string dateString) => {
                    if (dateString.Contains(date))
                    {
                        enteredFunction = true;
                        return DateTime.Parse(date);
                    }

                    return null;
                });

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(DateTime.Parse(date), result.ExpirationDate);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductName_SavesValidName()
        {
            // Arrange
            const string productName = "PRODUCT_NAME";
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLineValue(A<string>.That.Matches(x => x.Equals("LICENSE")), A<int>.That.Matches(x => x.Equals(2)), A<string[]>.Ignored))
                .Returns(productName);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(productName, result.ProductName);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductSeats_HandlesNullLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;
            const int shouldReturn = 0;
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.Matches(x => x.Equals("LICENSE")),
                A<int>.That.Matches(x => x.Equals(5)),
                A<string[]>.Ignored
            ))
            .Invokes(() => enteredFunction = true)
            .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(shouldReturn, result.Seats);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductSeats_HandlesEmptyLineValue()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;
            const int shouldReturn = 0;
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.Matches(x => x.Equals("LICENSE")),
                A<int>.That.Matches(x => x.Equals(5)),
                A<string[]>.Ignored
            ))
            .Invokes(() => enteredFunction = true)
            .Returns(string.Empty);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(shouldReturn, result.Seats);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductSeats_UncountedReturnsNegativeOne()
        {
            // Arrange
            const int shouldReturn = -1;
            bool enteredFunction = false;
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.Matches(x => x.Equals("LICENSE")),
                A<int>.That.Matches(x => x.Equals(5)),
                A<string[]>.Ignored
            ))
            .Invokes(() => enteredFunction = true)
            .Returns("uncounted");

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(shouldReturn, result.Seats);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductSeats_InvalidIntegerReturnsZero()
        {
            // Arrange
            const int shouldReturn = 0;
            bool enteredFunction = false;
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("LICENSE"),
                A<int>.That.IsEqualTo(5),
                A<string[]>.Ignored
            ))
            .Invokes(() => enteredFunction = true)
            .Returns("I_AM_NOT_A_NUMBER");

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(shouldReturn, result.Seats);
        }

        [Test]
        public void ProductLicenseFactory_Parse_GetProductSeats_ParsesValidInteger()
        {
            // Arrange 
            const string integer = "5";
            int shouldReturn = int.Parse(integer);
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("LICENSE"),
                A<int>.That.IsEqualTo(5),
                A<string[]>.Ignored
            ))
            .Returns(integer);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(shouldReturn, result.Seats);
        }
    }
}
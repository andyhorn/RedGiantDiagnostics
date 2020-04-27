using System;
using System.Collections.Generic;
using System.Linq;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class UtilitiesTests
    {
        private IUtilities _utilities;

        [SetUp]
        public void Setup()
        {
            _utilities = new Utilities();
        }

        [Test]
        public void UtilitiesTests_GetDateTimeFrom_NullDateStringReturnsNull()
        {
            // Arrange
            const string nullDateString = null;

            // Act
            var result = _utilities.GetDateTimeFrom(nullDateString);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetDateTimeFrom_EmptyDateStringReturnsNull()
        {
            // Arrange
            const string emptyDateString = "    ";

            // Act
            var result = _utilities.GetDateTimeFrom(emptyDateString);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetDateTimeFrom_InvalidStringReturnsNull()
        {
            // Arrange
            const string invalidDateString = "1234567890_Garbage";

            // Act
            var result = _utilities.GetDateTimeFrom(invalidDateString);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetDateTimeFrom_ValidDateStringReturnsDateTime()
        {
            // Arrange
            const string controlDateString = "01/01/2020 00:00:01";
            var date = DateTime.Parse(controlDateString);
            // var dateString = date.ToString();

            // Act
            var result = _utilities.GetDateTimeFrom(controlDateString);

            // Assert
            Assert.AreEqual(date, result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_NullSearchTermReturnsNull()
        {
            // Arrange
            const string nullSearchTerm = null;
            int validInt = A.Dummy<int>();
            string[] data = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetLineValue(nullSearchTerm, validInt, data);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_EmptySearchTermReturnsNull()
        {
            // Arrange
            const string emptySearchTerm = "    ";
            int validInt = A.Dummy<int>();
            string[] data = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetLineValue(emptySearchTerm, validInt, data);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_NullDataStringArrayReturnsNull()
        {
            // Arrange
            string searchTerm = A.Dummy<string>();
            int validInt = A.Dummy<int>();
            string[] data = null;

            // Act
            var result = _utilities.GetLineValue(searchTerm, validInt, data);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_EmptyDataStringArrayReturnsNull()
        {
            // Arrange
            string searchTerm = A.Dummy<string>();
            int validInt = A.Dummy<int>();
            string[] data = new string[0];

            // Act
            var result = _utilities.GetLineValue(searchTerm, validInt, data);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_NoMatchForSearchTermReturnsEmptyString()
        {
            // Arrange
            string searchTerm = "NoMatchForMe";
            int validInt = A.Dummy<int>();
            string[] data = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetLineValue(searchTerm, validInt, data);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_ValidMatch_InvalidIndex_ReturnsEmptyString()
        {
            // Arrange
            string searchTerm = "Match";
            string dataRow = $"{searchTerm} data data data";
            string[] data = new string[]
            {
                "data data data",
                dataRow,
                "data data data"
            };
            int index = dataRow.Split(" ").Length + 1;

            // Act
            var result = _utilities.GetLineValue(searchTerm, index, data);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void UtilitiesTests_GetLineValue_ValidMatch_ReturnsIndexValue()
        {
            // Arrange
            string searchTerm = "Match";
            string value = "hello";
            string dataRow = $"{searchTerm} {value}";
            string[] data = new string[]
            {
                "data data data",
                dataRow,
                "data data data"
            };
            int index = dataRow.Split(" ").ToList().IndexOf(value);

            // Act
            var result = _utilities.GetLineValue(searchTerm, index, data);

            // Assert
            Assert.AreEqual(value, result);
        }

        [Test]
        public void UtilitiesTests_MakeMac_NullStringReturnsNull()
        {
            // Arrange
            string nullString = null;

            // Act
            var result = _utilities.MakeMac(nullString);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_MakeMac_EmptyStringReturnsNull()
        {
            // Arrange
            string emptyString = "    ";

            // Act
            var result = _utilities.MakeMac(emptyString);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_MakeMac_RemovesSeparators()
        {
            // Arrange
            const string macString = "12 34.56:78-90_ab";
            const string finalString = "12:34:56:78:90:AB";

            // Act
            var result = _utilities.MakeMac(macString);

            // Assert
            Assert.AreEqual(finalString, result);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_NullBeginStringReturnsNull()
        {
            // Arrange
            const string nullString = null;
            string validString = A.Dummy<string>();
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetLinesBetween(nullString, validString, validStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_EmptyBeginstringReturnsNull()
        {
            // Arrange
            const string emptyString = "   ";
            string validString = A.Dummy<string>();
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetLinesBetween(emptyString, validString, validStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_NullStringArrayReturnsNull()
        {
            // Arrange
            string validString = A.Dummy<string>();
            string[] nullStringArray = null;

            // Act
            var result = _utilities.GetLinesBetween(validString, validString, nullStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_EmptyStringArrayReturnsNull()
        {
            // Arrange
            string validString = A.Dummy<string>();
            string[] emptyStringArray = new string[0];

            // Act
            var result = _utilities.GetLinesBetween(validString, validString, emptyStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_NoBeginMatchReturnsEmptyStringArray()
        {
            // Arrange
            const string beginMatch = "You'llNeverFindMe";
            string[] dataArray = new string[]
            {
                "data data data",
                "data data data",
                "data data data",
                "data data data"
            };

            // Act
            var result = _utilities.GetLinesBetween(beginMatch, null, dataArray);

            // Assert
            Assert.IsEmpty(result);
            Assert.IsInstanceOf(typeof(string[]), result);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_NoEndMarker_NonInclusive_ReturnsEverythingAfterBeginMarker()
        {
            // Arrange
            const string beginMatch = "Match";
            string[] dataArray = new string[]
            {
                "data data data",
                beginMatch,
                "data data data",
                "data data data",
                "data data data"
            };
            int correctLength = dataArray.Length - dataArray.ToList().IndexOf(beginMatch) - 1;

            // Act
            var result = _utilities.GetLinesBetween(beginMatch, null, dataArray);

            // Assert
            Assert.IsInstanceOf(typeof(string[]), result);
            Assert.AreEqual(correctLength, result.Length);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_NoEndMarker_Inclusive_ReturnsEverythingFromBeginMarkerToEnd()
        {
            // Arrange
            const string beginMatch = "Match";
            string[] dataArray = new string[]
            {
                "data data data",
                beginMatch,
                "data data data",
                "data data data",
                "data data data"
            };
            int correctLength = dataArray.Length - dataArray.ToList().IndexOf(beginMatch);

            // Act
            var result = _utilities.GetLinesBetween(beginMatch, null, dataArray, true);

            // Assert
            Assert.IsInstanceOf(typeof(string[]), result);
            Assert.AreEqual(correctLength, result.Length);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_HasEndMarker_NonInclusive_ReturnsEverythingBetweenMarkers()
        {
            // Arrange
            const string beginMatch = "beginMatch";
            const string endMatch = "endMatch";
            string[] dataArray = new string[]
            {
                "data data data",
                beginMatch,
                "data data data",
                "data data data",
                endMatch,
                "data data data"
            };
            int correctLength = dataArray.ToList().IndexOf(endMatch) - dataArray.ToList().IndexOf(beginMatch) - 1;

            // Act
            var result = _utilities.GetLinesBetween(beginMatch, endMatch, dataArray, false);

            // Assert
            Assert.IsInstanceOf(typeof(string[]), result);
            Assert.AreEqual(correctLength, result.Length);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_HasEndMarker_Inclusive_ReturnsEverythingBetweenMarkersPlusMarkers()
        {
            // Arrange
            const string beginMatch = "beginMatch";
            const string endMatch = "endMatch";
            string[] dataArray = new string[]
            {
                "data data data",
                beginMatch,
                "data data data",
                "data data data",
                "data data data",
                endMatch,
                "data data data"
            };
            int correctLength = dataArray.ToList().IndexOf(endMatch) - dataArray.ToList().IndexOf(beginMatch) + 1;

            // Act
            var result = _utilities.GetLinesBetween(beginMatch, endMatch, dataArray, true);

            // Assert
            Assert.IsInstanceOf(typeof(string[]), result);
            Assert.AreEqual(correctLength, result.Length);
        }

        [Test]
        public void UtilitiesTests_GetLinesBetween_ReturnsActualContentBetweenMarkers()
        {
            // Arrange
            const string garbage = "data data data";
            const string beginMatch = "beginMatch";
            const string endMatch = "endMatch";
            string[] content = new string[]
            {
                "content line 1",
                "content line 2",
                "content line 3"
            };
            List<string> data = new List<string>();
            data.Add(garbage);
            data.Add(beginMatch);
            data.AddRange(content);
            data.Add(endMatch);
            data.Add(garbage);

            // Act
            var result = _utilities.GetLinesBetween(beginMatch, endMatch, data.ToArray());

            // Assert
            Assert.IsInstanceOf(typeof(string[]), result);
            Assert.AreEqual(content.Length, result.Length);

            bool containsContent = true;
            for (var i = 0; i < result.Length; i++)
            {
                if (content[i] != result[i])
                {
                    containsContent = false;
                    break;
                }
            }
            Assert.IsTrue(containsContent);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_NullBeginMarker_ReturnsNull()
        {
            // Arrange
            const string nullString = null;
            string validString = A.Dummy<string>();
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetSubsections(nullString, validString, validStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_EmptyBeginMarker_ReturnsNull()
        {
            // Arrange
            const string emptyString = "      ";
            string validString = A.Dummy<string>();
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetSubsections(emptyString, validString, validStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_NullEndMarker_ReturnsNull()
        {
            // Arrange
            const string nullString = null;
            string validString = A.Dummy<string>();
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetSubsections(validString, nullString, validStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_EmptyEndMarker_ReturnsNull()
        {
            // Arrange
            const string emptyString = "      ";
            string validString = A.Dummy<string>();
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetSubsections(validString, emptyString, validStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_NullDataArray_ReturnsNull()
        {
            // Arrange
            string validString = A.Dummy<string>();
            string[] nullStringArray = null;

            // Act
            var result = _utilities.GetSubsections(validString, validString, nullStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_EmptyDataArray_ReturnsNull()
        {
            // Arrange
            string validString = A.Dummy<string>();
            string[] emptyStringArray = new string[0];

            // Act
            var result = _utilities.GetSubsections(validString, validString, emptyStringArray);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_NoBeginMarkerMatch_ReturnsEmptyCollection()
        {
            // Arrange
            const string beginMarker = "beginMarker";
            string endMarker = A.Dummy<string>();
            string[] dataArray = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetSubsections(beginMarker, endMarker, dataArray);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_NoEndMarkerMatch_ReturnsOneListOfAllContentFollowingBeginMarker()
        {
            // Arrange
            const string garbage = "data data data";
            const string beginMarker = "beginMarker";
            string endMarker = "NoMatchForMe";
            string[] content = new string[3]
            {
                "content content content",
                "content content content",
                "content content content"
            };
            List<string> data = new List<string>();
            data.Add(garbage);
            data.Add(beginMarker);
            data.AddRange(content);

            // Act
            var result = _utilities.GetSubsections(beginMarker, endMarker, data.ToArray());

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void UtilitiesTests_GetSubsections_ReturnsContentBetweenMarkers_IncludingBeginMarker()
        {
            // Arrange
            const string garbage = "data data data";
            const string beginMarker = "beginMarker";
            const string endMarker = "endMarker";
            string[] content = new string[3]
            {
                "content content content",
                "content content content",
                "content content content"
            };
            List<string> data = new List<string>();
            data.Add(garbage);
            data.Add(beginMarker);
            data.AddRange(content);
            data.Add(endMarker);
            data.Add(garbage);

            // Act
            var result = _utilities.GetSubsections(beginMarker, endMarker, data.ToArray());

            // Assert
            Assert.AreEqual(1, result.Count());

            Assert.AreEqual(content.Length + 1, result.ToList()[0].Count());
        }

        [Test]
        public void UtilitiesTests_GetSubsections_ReturnsCorrectNumberOfSubsections()
        {
            // Arrange
            const string garbage = "data data data";
            const string beginMarker = "beginMarker";
            const string endMarker = "endMarker";
            string[] content = new string[3]
            {
                "content content content",
                "content content content",
                "content content content"
            };
            List<string> data = new List<string>();
            data.Add(garbage);
            data.Add(garbage);
            data.Add(beginMarker);
            data.AddRange(content);
            data.Add(endMarker);
            data.Add(beginMarker);
            data.AddRange(content);
            data.Add(endMarker);

            // Act
            var result = _utilities.GetSubsections(beginMarker, endMarker, data.ToArray());

            // Assert
            Assert.AreEqual(2, result.ToList().Count);
        }

        [Test]
        public void UtilitiesTests_GetSubsections_SameEndMarkerAndBeginMarker_ReturnsCorrectSubsections()
        {
            // Arrange
            const string marker = "Marker";
            const string garbage = "data data data";
            string[] content = new string[3]
            {
                "content content content",
                "content content content",
                "content content content"
            };
            List<string> data = new List<string>();

            data.Add(garbage);
            data.Add(marker);
            data.AddRange(content);
            data.Add(marker);
            data.AddRange(content);
            data.Add(marker);
            data.AddRange(content);

            // Act
            var result = _utilities.GetSubsections(marker, marker, data.ToArray());

            // Assert
            Assert.AreEqual(3, result.Count());

            bool subsectionsCorrectLength = true;
            foreach (var section in result)
            {
                if (section.Count() != content.Length + 1)
                {
                    subsectionsCorrectLength = false;
                    break;
                }
            }
            Assert.IsTrue(subsectionsCorrectLength);
        }

        [Test]
        public void UtilitiesTests_GetAfter_NullLineIdentifier_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _utilities.GetAfter(null, A.Dummy<string>(), A.CollectionOfDummy<string>(1).ToArray());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_EmptyLineIdentifier_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _utilities.GetAfter(string.Empty, A.Dummy<string>(), A.CollectionOfDummy<string>(1).ToArray());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_NullSeparator_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _utilities.GetAfter(A.Dummy<string>(), null, A.CollectionOfDummy<string>(1).ToArray());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_EmptySeparator_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _utilities.GetAfter(A.Dummy<string>(), string.Empty, A.CollectionOfDummy<string>(5).ToArray());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_NullData_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _utilities.GetAfter(A.Dummy<string>(), A.Dummy<string>(), null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_EmptyData_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _utilities.GetAfter(A.Dummy<string>(), A.Dummy<string>(), new string[0]);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_NoMatch_ReturnsNull()
        {
            // Arrange
            const string searchTerm = "NoMatchForMe";
            var data = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetAfter(searchTerm, A.Dummy<string>(), data);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_NoSeparatorMatch_ReturnsFullLine()
        {
            // Arrange
            string searchTerm = A.Dummy<string>();
            const string separator = "NoMatchForMe";
            var data = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _utilities.GetAfter(searchTerm, separator, data);

            // Assert
            Assert.AreEqual(searchTerm, result);
        }

        [Test]
        public void UtilitiesTests_GetAfter_ReturnsPortionAfterSeparator()
        {
            // Arrange
            const string searchTerm = "SearchTerm";
            const string separator = ":";
            const string returnValue = "ReturnMe";
            string line = searchTerm + separator + returnValue;
            string[] data = new string[] { line };

            // Act
            var result = _utilities.GetAfter(searchTerm, separator, data);

            // Assert
            Assert.AreEqual(returnValue, result);
        }
    }
}
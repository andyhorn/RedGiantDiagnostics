using System;
using System.Linq;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class StatisticsParserTests
    {
        private IStatisticsParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new StatisticsParser();
        }

        [Test]
        public void StatisticsParser_ParseTableDates_NullStringArray_ReturnsEmptyDateTimeArray()
        {
            // Arrange
            const string[] nullStringArray = null;

            // Act
            var result = _parser.ParseTableDates(nullStringArray);

            // Assert
            Assert.IsEmpty(result);
            Assert.IsInstanceOf(typeof(DateTime[]), result);
        }

        [Test]
        public void StatisticsParser_ParseTableDates_EmptyStringArray_ReturnsEmptyDateTimeArray()
        {
            // Arrange
            string[] emptyStringArray = new string[0];

            // Act
            var result = _parser.ParseTableDates(emptyStringArray);

            // Assert
            Assert.IsEmpty(result);
            Assert.IsInstanceOf(typeof(DateTime[]), result);
        }

        [Test]
        public void StatisticsParser_ParseTableDates_NoStartTimeRow_ReturnsEmptyDateTimeArray()
        {
            // Arrange
            string[] noStartTimeArray = new string[]
            {
                "Some text here",
                "more text here",
                "another line of text",
                "one final line of text"
            };

            // Act
            var result = _parser.ParseTableDates(noStartTimeArray);

            // Assert
            Assert.IsEmpty(result);
            Assert.IsInstanceOf(typeof(DateTime[]), result);
        }

        [Test]
        public void StatisticsParser_ParseTableDates_ParsesDates()
        {
            // Arrange
            const string dateOne = "4/22";
            const string dateTwo = "4/23";
            const string dateThree = "4/23";
            const string timeOne = "07:51:07";
            const string timeTwo = "00:00:01";
            const string timeThree = "09:08:50";

            var dateTimeOne = DateTime.Parse($"{dateOne}/{DateTime.Now.Year} {timeOne}");
            var dateTimeTwo = DateTime.Parse($"{dateTwo}/{DateTime.Now.Year} {timeTwo}");
            var dateTimeThree = DateTime.Parse($"{dateThree}/{DateTime.Now.Year} {timeThree}");

            var dateTimeArray = new DateTime[]
            {
                dateTimeOne,
                dateTimeTwo,
                dateTimeThree
            };

            string dateString = $"Start time  {dateOne} {timeOne}  {dateTwo} {timeTwo}  {dateThree} {timeThree}";
            string[] dataArray = new string[]
            {
                "some text here",
                "more text",
                dateString,
                "more text",
                "another line",
                "one last line"
            };

            // Act
            var result = _parser.ParseTableDates(dataArray);

            // Assert
            Assert.AreEqual(dateTimeArray, result);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_NullStringArray_ReturnsIntArrayOfZeroes()
        {
            // Arrange
            string[] nullStringArray = null;
            string dummyRowHeader = A.Dummy<string>();

            // Act
            var result = _parser.GetColumnValues(dummyRowHeader, nullStringArray);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);
            
            bool areAllZero = result.ToList().All(x => x == 0);
            Assert.IsTrue(areAllZero);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_EmptyStringArray_ReturnsIntArrayOfZeroes()
        {
            // Arrange
            string[] emptyStringArray = new string[0];
            string dummyRowHeader = A.Dummy<string>();

            // Act
            var result = _parser.GetColumnValues(dummyRowHeader, emptyStringArray);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);

            bool areAllZero = result.ToList().All(x => x == 0);

            Assert.IsTrue(areAllZero);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_NullRowHeader_ReturnsIntArrayOfZeroes()
        {
            // Arrange
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();
            string nullRowHeader = null;

            // Act
            var result = _parser.GetColumnValues(nullRowHeader, validStringArray);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);

            bool areAllZero = result.ToList().All(x => x == 0);

            Assert.IsTrue(areAllZero);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_WhitespaceRowHeader_ReturnsIntArrayOfZeroes()
        {
            // Arrange
            string[] validStringArray = A.CollectionOfDummy<string>(5).ToArray();
            string whitespaceRowHeader = "     ";

            // Act
            var result = _parser.GetColumnValues(whitespaceRowHeader, validStringArray);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);

            bool areAllZero = result.ToList().All(x => x == 0);

            Assert.IsTrue(areAllZero);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_ParsesOnlyOneColumn()
        {
            // Arrange
            const string rowHeader = "Row Header:";
            const string filler = "( 0/sec)";
            const int valueOne = 10;
            string dataRow = $"{rowHeader} {valueOne} {filler}";
            string[] data = new string[]
            {
                "data data data",
                "data data data",
                dataRow,
                "data data data"
            };

            // Act
            var result = _parser.GetColumnValues(rowHeader, data);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);

            Assert.AreEqual(valueOne, result[0]);
            Assert.AreEqual(0, result[1]);
            Assert.AreEqual(0, result[2]);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_ParsesTwoColumns()
        {
            // Arrange
            const string rowHeader = "Row Header:";
            const string filler = "( 0/sec)";
            const int valueOne = 10;
            const int valueTwo = 20;
            string dataRow = $"{rowHeader} {valueOne} {filler} {valueTwo} {filler}";
            string[] data = new string[]
            {
                "data data data",
                "data data data",
                dataRow,
                "data data data"
            };

            // Act
            var result = _parser.GetColumnValues(rowHeader, data);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);

            Assert.AreEqual(valueOne, result[0]);
            Assert.AreEqual(valueTwo, result[1]);
            Assert.AreEqual(0, result[2]);
        }

        [Test]
        public void StatisticsParser_GetColumnValues_ValidData_ReturnsValidCounts()
        {
            // Arrange
            const string rowHeader = "Row Header:";
            const string filler = "( 0/sec)";
            const int valueOne = 10;
            const int valueTwo = 20;
            const int valueThree = 30;
            string dataRow = $"{rowHeader} {valueOne} {filler} {valueTwo} {filler} {valueThree} {filler}";
            string[] validData = new string[]
            {
                "data data data",
                "text here text here",
                dataRow,
                "text here and here",
                "more data and junk"
            };

            // Act
            var result = _parser.GetColumnValues(rowHeader, validData);

            // Assert
            Assert.IsInstanceOf(typeof(int[]), result);

            Assert.AreEqual(valueOne, result[0]);
            Assert.AreEqual(valueTwo, result[1]);
            Assert.AreEqual(valueThree, result[2]);
        }
    }
}
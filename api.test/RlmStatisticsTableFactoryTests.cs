using System;
using System.Linq;
using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class RlmStatisticsTableFactoryTests
    {
        private IUtilities _utilities;
        private IServerStatusFactory _serverStatusFactory;
        private IStatisticsParser _statisticsParser;
        private RlmStatisticsTableFactory _factory;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();
            _serverStatusFactory = A.Fake<IServerStatusFactory>();
            _statisticsParser = A.Fake<IStatisticsParser>();

            _factory = new RlmStatisticsTableFactory(
                _utilities,
                _serverStatusFactory,
                _statisticsParser
            );
        }

        [Test]
        public void RlmStatisticsTableFactory_New_ReturnsNewRlmStatisticsTableObject()
        {
            // Arrange

            // Act
            var result = _factory.New;

            // Assert
            Assert.IsInstanceOf(typeof(RlmStatisticsTable), result);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_NullDataReturnsNullObject()
        {
            // Arrange

            // Act
            var result = _factory.Parse(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_EmptyDataReturnsNullObject()
        {
            // Arrange

            // Act
            var result = _factory.Parse(new string[0]);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServerName_NullLineValueReturnslEmptyString()        
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("Status for \"rlm\""),
                A<int>.That.IsEqualTo(2),
                A<string[]>.Ignored
            ))
            .Returns(null);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.ServerName);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServerName_EmptyLineValueReturnsEmptyString()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("Status for \"rlm\""),
                A<int>.That.IsEqualTo(2),
                A<string[]>.Ignored
            ))
            .Returns("     ");

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.ServerName);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServerName_StripsQuotationMarksFromName()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("Status for \"rlm\""),
                A<int>.That.IsEqualTo(2),
                A<string[]>.Ignored
            ))
            .Returns("\"rlm\"");

            // Act
            var result = _factory.Parse(data);

            // Assert
            var stripped = !result.ServerName.Contains("\"");
            Assert.IsTrue(stripped);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServerName_SavesServerNameWithoutQuotations()
        {
            // Arrange
            const string name = "rlm";
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLineValue(
                A<string>.That.IsEqualTo("Status for \"rlm\""),
                A<int>.That.IsEqualTo(2),
                A<string[]>.Ignored
            ))
            .Returns(name);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(name, result.ServerName);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetStartTimes_SavesEmptyDateTimeArray()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _statisticsParser.ParseTableDates(
                A<string[]>.Ignored
            ))
            .Returns(new DateTime[0]);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.StartTimes);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetStartTimes_SavesDateTimeArray()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            DateTime[] dates = new DateTime[]
            {
                DateTime.Now,
                DateTime.Now,
                DateTime.Now
            };
            A.CallTo(() => _statisticsParser.ParseTableDates(
                A<string[]>.Ignored
            ))
            .Returns(dates);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(dates, result.StartTimes);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetMessages_SavesReturnedIntegerArray()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            int[] ints = new int[]
            {
                1,
                2,
                3
            };
            A.CallTo(() => _statisticsParser.GetColumnValues(
                A<string>.That.IsEqualTo("Messages:"),
                A<string[]>.Ignored
            ))
            .Returns(ints);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(ints, result.Messages);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetConnections_SavesReturnedIntegerArray()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            int[] ints = new int[]
            {
                1,
                2,
                3
            };
            A.CallTo(() => _statisticsParser.GetColumnValues(
                A<string>.That.IsEqualTo("Connections:"),
                A<string[]>.Ignored
            ))
            .Returns(ints);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(ints, result.Connections);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServers_NoServersSavesEmptyCollection()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLinesBetween(
                A<string>.That.IsEqualTo("ISV Servers"),
                A<string>.That.Matches(x => x.Contains("====")),
                A<string[]>.Ignored,
                A<bool>.Ignored
            ))
            .Returns(new string[0]);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.Servers);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServers_RemovesHeaderColumnIfPresent()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            string[] dataLines = new string[]
            {
                "Name port Running Restarts",
                "test test test test"
            };
            A.CallTo(() => _utilities.GetLinesBetween(
                A<string>.That.IsEqualTo("ISV Servers"),
                A<string>.That.Matches(x => x.Contains("=====")),
                A<string[]>.Ignored,
                A<bool>.Ignored
            ))
            .Returns(dataLines);

            // Act
            var result = _factory.Parse(data);

            // Assert
            A.CallTo(() => _serverStatusFactory.Parse(
                A<string>.Ignored
            ))
            .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServers_LinesNotRemovedIfHeaderNotPresent()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            string[] dataLines = new string[]
            {
                "test test test test",
                "test test test test"
            };
            A.CallTo(() => _utilities.GetLinesBetween(
                A<string>.That.IsEqualTo("ISV Servers"),
                A<string>.That.Matches(x => x.Contains("=====")),
                A<string[]>.Ignored,
                A<bool>.Ignored
            ))
            .Returns(dataLines);

            // Act
            var result = _factory.Parse(data);

            // Assert
            A.CallTo(() => _serverStatusFactory.Parse(A<string>.Ignored))
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServers_NullServerStatusObjectsNotAddedToList()
        {
            // Arrange
            var data = A.CollectionOfDummy<string>(5).ToArray();
            string[] dataLines = A.CollectionOfDummy<string>(2).ToArray();
            ServerStatus[] factoryResults = new ServerStatus[]
            {
                null,
                null
            };
            A.CallTo(() => _utilities.GetLinesBetween(
                A<string>.That.IsEqualTo("ISV Servers"),
                A<string>.That.Matches(x => x.Contains("=====")),
                A<string[]>.Ignored, 
                A<bool>.Ignored
            ))
            .Returns(dataLines);
            A.CallTo(() => _serverStatusFactory.Parse(A<string>.Ignored))
                .ReturnsNextFromSequence(factoryResults);

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.IsEmpty(result.Servers);
        }

        [Test]
        public void RlmStatisticsTableFactory_Parse_GetServers_ServersAddedToList()
        {
            // Arrange
            const int numServers = 2;
            var data = A.CollectionOfDummy<string>(5).ToArray();
            A.CallTo(() => _utilities.GetLinesBetween(
                A<string>.That.IsEqualTo("ISV Servers"),
                A<string>.That.Matches(x => x.Contains("=====")),
                A<string[]>.Ignored,
                A<bool>.Ignored
            ))
            .Returns(A.CollectionOfDummy<string>(numServers).ToArray());
            A.CallTo(() => _serverStatusFactory.Parse(A<string>.Ignored))
                .Returns(A.Fake<ServerStatus>());

            // Act
            var result = _factory.Parse(data);

            // Assert
            Assert.AreEqual(numServers, result.Servers.Count());
        }
    }
}
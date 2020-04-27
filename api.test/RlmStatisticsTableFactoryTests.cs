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
    }
}
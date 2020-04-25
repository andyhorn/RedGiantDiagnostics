using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using API.Entities;
using API.Factories;
using API.Helpers;
using FakeItEasy;
using NUnit.Framework;

namespace api.test
{
    public class LogParserTests
    {
        private string[] validMetadata = new string[]
        {
            "RLM Server Diagnostics at 04/23/2020 09:14",
            "RLM version: 11.0BL2",
            "RLM platform: x64_w3",
            "OS version: 6.1",
            "ISV name: rlm",
            "Hostname: WBWTMZPJAPP05V",
            "User: system",
            "Working directory: C:\\Program Files\\Red Giant\\License Server",
            "Environment:",
            "HTTP_PROXY=<not set>",
            "RLM_CONNECT_TIMEOUT=<not set>",
            "RLM_EXTENDED_ERROR_MESSAGES=1",
            "RLM_LICENSE=<not set>",
            "RLM_NO_UNLIMIT=<not set>",
            "RLM_PATH_RANDOMIZE=<not set>",
            "RLM_PROJECT=<not set>",
            "RLM_QUEUE=<not set>",
            "RLM_ROAM=<not set>",
            "RLMSTAT=<not set>",
            "rlm_LICENSE=<not set>",
            "RLM hostid list:",
            "bc001e85 00155db1d91b 00155db1d91c ip=10.146.176.79 ip=10.78.78.153",
            "License files:",
            "RedGiant-RLM-2986dbcb-492f-49da-88c2-0a3fb465128f.lic"
        };
        private IUtilities _utilities;
        private IDebugLogFactory _debugLogFactory;
        private IIsvStatisticsFactory _isvStatisticsFactory;
        private ILicenseFileFactory _licenseFileFactory;
        private IRlmStatisticsTableFactory _rlmStatisticsTableFactory;
        private IRlmInstanceFactory _rlmInstanceFactory;
        private ILogParser _logParser;

        [SetUp]
        public void Setup()
        {
            _utilities = A.Fake<IUtilities>();
            _debugLogFactory = A.Fake<IDebugLogFactory>();
            _isvStatisticsFactory = A.Fake<IIsvStatisticsFactory>();
            _licenseFileFactory = A.Fake<ILicenseFileFactory>();
            _rlmStatisticsTableFactory = A.Fake<IRlmStatisticsTableFactory>();
            _rlmInstanceFactory = A.Fake<IRlmInstanceFactory>();

            _logParser = new LogParser(
                _utilities,
                _debugLogFactory,
                _isvStatisticsFactory,
                _licenseFileFactory,
                _rlmStatisticsTableFactory,
                _rlmInstanceFactory
            );
        }

        [Test]
        public void LogParser_Parse_NullLogFileObject_ThrowsException()
        {
            // Arrange
            LogFile nullObject = null;
            var data = A.CollectionOfDummy<string>(5).ToArray();

            // Act and Assert
            Assert.Throws(typeof(ArgumentNullException), () => _logParser.Parse(nullObject, data));
        }

        [Test]
        public void LogParser_Parse_NullDataArray_ThrowsException()
        {
            // Arrange
            LogFile log = A.Fake<LogFile>();
            string[] nullData = null;

            // Act and Assert
            Assert.Throws(typeof(ArgumentNullException), () => _logParser.Parse(log, nullData));
        }

        [Test]
        public void LogParser_Parse_EmptyDataArray_ThrowsException()
        {
            // Arrange
            LogFile log = A.Fake<LogFile>();
            string[] emptyData = new string[0];

            // Act and Assert
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => _logParser.Parse(log, emptyData));
        }

        [Test]
        public void LogParser_Parse_ParseMetadata_HandlesInvalidData()
        {
            // Arrange
            LogFile file = A.Fake<LogFile>();
            string[] data = A.CollectionOfDummy<string>(5).ToArray();

            // Act
            var result = _logParser.Parse(file, data);

            // Assert
            Assert.IsNull(result.Date);
            Assert.IsEmpty(result.RlmVersion);
            Assert.IsEmpty(result.Hostname);
        }

        [Test]
        public void LogParser_Parse_ParseMetadata_ReturnsValid()
        {
            // Arrange
            var file = A.Dummy<LogFile>();
            const string dateString = "04/23/2020";
            const string timeString = "09:14";
            DateTime date = DateTime.Parse(dateString);
            const string rlmVersion = "11.0BL2";
            const string hostname = "WBWTMZPJAPP05V";

            string[] lineValues = new string[]
            {
                dateString,
                timeString,
                rlmVersion,
                hostname
            };

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("RLM Server Diagnostics") && end.Contains("Environment"))
                        return A.CollectionOfDummy<string>(10).ToArray();
                    else
                        return null;
                });

            A.CallTo(() => _utilities.GetLineValue(A<string>.Ignored, A<int>.Ignored, A<string[]>.Ignored))
                .ReturnsNextFromSequence(lineValues);

            A.CallTo(() => _utilities.GetDateTimeFrom(A<string>.Ignored))
                .Returns(date);

            // Act
            var result = _logParser.Parse(file, A.CollectionOfDummy<string>(10).ToArray());

            // Assert
            Assert.AreEqual(date, result.Date);
            Assert.AreEqual(rlmVersion, result.RlmVersion);
            Assert.AreEqual(hostname, result.Hostname);
        }

        [Test]
        public void LogParser_Parse_ParseEnvironmentVariables_HandlesNullLines()
        {
            // Arrange
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool hitEnvironmentVariables = false;
            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("Environment:") && end.Contains("RLM hostid list:"))
                        hitEnvironmentVariables = true;
                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(hitEnvironmentVariables);
            Assert.IsEmpty(result.EnvironmentVariables);
        }

        [Test]
        public void LogParser_Parse_ParseEnvironmentVariables_HandlesSplitFail()
        {
            // Arrange
            string[] returnArray = new string[] { "datadatadata", "datadatadata" };
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool hitEnvironmentVariables = false;
            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("Environment:") && end.Contains("RLM hostid list:"))
                    {
                        hitEnvironmentVariables = true;
                        return returnArray;
                    }
                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(hitEnvironmentVariables);
            Assert.IsEmpty(result.EnvironmentVariables);
        }

        [Test]
        public void LogParser_Parse_ParseEnvironmentVariables_ParsesCorrectly()
        {
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(10).ToArray();

            const string keyOne = "KeyOne";
            const string keyTwo = "KeyTwo";
            const string valueOne = "ValueOne";
            const string valueTwo = "ValueTwo";

            var returnArray = new string[]
            {
                $"{keyOne}={valueOne}",
                $"{keyTwo}={valueTwo}"
            };

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("Environment:") && end.Contains("RLM hostid list"))
                    {
                        return returnArray;
                    }

                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.AreEqual(valueOne, result.EnvironmentVariables[keyOne.ToString()]);
            Assert.AreEqual(valueTwo, result.EnvironmentVariables[keyTwo.ToString()]);
        }

        [Test]
        public void LogParser_Parse_ParseHostMacAndIpList_HandlesNullData()
        {
            // Arrange
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("RLM hostid list") && end.Contains("License files"))
                    {
                        enteredFunction = true;
                    }

                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsEmpty(result.HostIpList);
            Assert.IsEmpty(result.HostMacList);
        }

        [Test]
        public void LogParser_Parse_ParseHostIpAndMacList_HandlesInvalidLineData()
        {
            // Arrange
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;
            const string invalidDataLine = "InvalidDataInvalidDataInvalidData";

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("RLM hostid list") && end.Contains("License files"))
                    {
                        enteredFunction = true;
                        return new string[] { invalidDataLine };
                    }

                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsEmpty(result.HostMacList);
            Assert.IsEmpty(result.HostIpList);
        }

        [Test]
        public void LogParser_Parse_ParseHostIpAndMacList_ParsesMac()
        {
            // Arrange
            const string rawMacOne = "1234567890ab";
            const string rawMacTwo = "ba0987654321";
            const string formattedMacOne = "12:34:56:78:90:AB";
            const string formattedMacTwo = "BA:09:87:65:43:21";
            string[] macList = new string[] { $"{rawMacOne} {rawMacTwo}" };

            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();
            bool enteredFunction = false;

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("RLM hostid list") && end.Contains("License files"))
                    {
                        enteredFunction = true;
                        return macList;
                    }

                    return null;
                });

            A.CallTo(() => _utilities.MakeMac(A<string>.Ignored))
                .ReturnsLazily((string input) => {
                    switch (input)
                    {
                        case rawMacOne:
                            return formattedMacOne;
                        case rawMacTwo:
                            return formattedMacTwo;
                        default:
                            return null;
                    }
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(formattedMacOne, result.HostMacList.ElementAt(0));
            Assert.AreEqual(formattedMacTwo, result.HostMacList.ElementAt(1));
        }

        [Test]
        public void LogParser_Parse_ParseHostIpAndMacList_ParsesIps()
        {
            // Arrange
            bool enteredFunction = false;

            const string rawIpOne = "ip=123.456.789";
            const string rawIpTwo = "ip=1.1.1";
            const string cleanIpOne = "123.456.789";
            const string cleanIpTwo = "1.1.1";
            string[] ipList = new string[] { $"{rawIpOne} {rawIpTwo}" };

            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("RLM hostid list") && end.Contains("License files"))
                    {
                        enteredFunction = true;
                        return ipList;
                    }

                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.AreEqual(cleanIpOne, result.HostIpList.ElementAt(0));
            Assert.AreEqual(cleanIpTwo, result.HostIpList.ElementAt(1));
        }

        [Test]
        public void LogParser_Parse_ParseLicenseFiles_HandlesNullLicenseLines()
        {
            // Arrange
            bool enteredFunction = false;
            bool wentTooFar = false;
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("LICENSE FILE") && end.Contains("RLM Options"))
                    {
                        enteredFunction = true;
                    }

                    return null;
                });

            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data) => {
                    if (begin.Contains("LICENSE FILE") && end.Contains("LICENSE FILE"))
                    {
                        wentTooFar = true;
                    }

                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsFalse(wentTooFar);
            Assert.IsEmpty(result.Licenses);
        }

        [Test]
        public void LogParser_Parse_ParseLicenseFiles_HandlesEmptyLicenseSection()
        {
            // Arrange
            bool enteredFunction = false;
            bool wentTooFar = false;
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data, bool inclusive) => {
                    if (begin.Contains("LICENSE FILE") && end.Contains("RLM Options"))
                    {
                        enteredFunction = true;
                        return new string[0];
                    }

                    return null;
                });
            A.CallTo(() => _utilities.GetSubsections(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored))
                .ReturnsLazily((string begin, string end, string[] data) => {
                    if (begin.Contains("LICENSE FILE") && end.Contains("LICENSE FILE"))
                    {
                        wentTooFar = true;
                    }

                    return null;
                });

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.IsTrue(enteredFunction);
            Assert.IsFalse(wentTooFar);
            Assert.IsEmpty(result.Licenses);
        }

        [Test]
        public void LogParser_Parse_EntersNewThread()
        {
            // Arrange
            var log = A.Fake<LogFile>();
            var data = A.CollectionOfDummy<string>(5).ToArray();

            var currentThread = Thread.CurrentThread.ManagedThreadId;
            int secondThreadId = currentThread;

            A.CallTo(() => _utilities.GetLinesBetween(A<string>.Ignored, A<string>.Ignored, A<string[]>.Ignored, A<bool>.Ignored))
                .Invokes(() => secondThreadId = Thread.CurrentThread.ManagedThreadId)
                .Returns(null);

            // Act
            var result = _logParser.Parse(log, data);

            // Assert
            Assert.AreNotEqual(currentThread, secondThreadId);
        }
    }
}
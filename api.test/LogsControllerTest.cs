using NUnit.Framework;
using FakeItEasy;
using API.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.test
{
    public class LogsControllerTests
    {
        private LogsController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new LogsController();
        }

        [Test]
        public void LogsController_Get()
        {
            // Get the test endpiont
            var result = _controller.Get();

            // Ensure it returns an Ok
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            
            // Get the content from the result
            var data = result as OkObjectResult;
            var content = (string)data.Value;

            // Ensure it contains the "Hello world!" string
            Assert.AreEqual("Hello world!", content);
        }

        [Test]
        public void LogsController_GetById()
        {
            
        }
    }
}
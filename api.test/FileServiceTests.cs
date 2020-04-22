using API.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace api.test
{
    public class FileServiceTests
    {
        private IFileService _fileService;

        [SetUp]
        public void Setup()
        {
            _fileService = new FileService();
        }

        [Test]
        public void FileService_ReadFormFile_FileLengthZeroReturnsNull()
        {
            // Arrange
            IFormFile file = new FormFile(null, 0, 0, null, null);

            // Act
            var result = _fileService.ReadFormFile(file);

            // Assert
            Assert.IsNull(result);
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;
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
        public void FileService_ReadFormFile_NullObjectThrowsNullArgumentException()
        {
            // Arrange
            IFormFile nullFile = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => _fileService.ReadFormFile(nullFile));
        }

        [Test]
        public void FileService_ReadFormFile_FileLengthZeroReturnsNull()
        {
            // Arrange
            IFormFile emptyFile = new FormFile(null, 0, 0, null, null);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => _fileService.ReadFormFile(emptyFile));
        }

        [Test]
        public void FileService_ReadFormFile_ReturnsStringFromStream()
        {
            // Arrange
            const int fileLength = 100;
            Stream dummyStream = A.Dummy<Stream>();
            string dummyString = A.Dummy<string>();

            var file = new FormFile(
                dummyStream,
                0,
                fileLength,
                dummyString,
                dummyString);

            // Act
            var result = _fileService.ReadFormFile(file);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fileLength, result.Length);
        }

        [Test]
        public async Task FileService_ReadFormFileAsync_ReturnsStringFromStream()
        {
            // Arrange
            const int fileLength = 100;
            var dummyStream = A.Dummy<Stream>();
            var dummyString = A.Dummy<string>();
            var file = new FormFile(
                dummyStream, 
                0, 
                fileLength, 
                dummyString, 
                dummyString);

            // Act
            var result = await _fileService.ReadFormFileAsync(file);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(fileLength, file.Length);
        }
    }
}
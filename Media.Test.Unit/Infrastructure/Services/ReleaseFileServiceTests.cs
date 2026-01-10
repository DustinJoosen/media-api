using Media.Abstractions.Interfaces;
using Media.Core.Exceptions;
using Media.Test.Core.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Media.Test.Unit.Infrastructure.Services
{
    [TestClass]
    public class ReleaseFileServiceTests : TestWithInMemoryDb
    {
        private IFileService _fileService;
        private string _rootPath;

        [TestInitialize]
        public void Setup()
        {
            this.BaseSetup();

            /*
             * Yeah, yeah. I know it's not a ReleaseFileService as the Test class name suggests, 
             * but it inherits from the ReleaseFileService. The ONLY difference is the root folder.
             * Don't want the tests to spam my development or release folder.
             */
            this._fileService = new TestingFileService();
            this._rootPath = Path.Combine(Path.GetTempPath(), "media-api-tests");
        }

        [TestMethod]
        public async Task UploadFile_ShouldCreateFile_WhenValidFormFileProvided()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var file = this.CreateFakeFormFile();

            var expectedFolder = Path.Combine(this._rootPath,
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));
            var expectedFilePath = Path.Combine(expectedFolder, "test.jpg");

            // Act.
            await this._fileService.UploadFile(id, file);

            // Assert.
            Assert.IsTrue(Directory.Exists(expectedFolder));
            Assert.IsTrue(File.Exists(expectedFilePath));
        }

        [TestMethod]
        public async Task UploadFile_ShouldThrowBadRequestException_WhenFormFileIsNull()
        {
            // Arrange.
            var id = Guid.NewGuid();

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<BadRequestException>(async () =>
                await this._fileService.UploadFile(id, null!)
            );

            // Assert.
            Assert.AreEqual("Uploaded file is null or empty.", ex.Message);
        }

        [TestMethod]
        public async Task UploadFile_ShouldThrowBadRequestException_WhenFormFileLengthIsZero()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var stream = new MemoryStream(); // (Length = 0).
            var file = new FormFile(stream, 0, 0, "file", "empty.jpg");

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<BadRequestException>(async () =>
                await this._fileService.UploadFile(id, file)
            );

            // Assert.
            Assert.AreEqual("Uploaded file is null or empty.", ex.Message);
        }

        [TestMethod]
        public void GetFileStreamPreview_ShouldReturnFileStream_WhenFileIsPreviewable()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var folder = Path.Combine(this._rootPath, 
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));

            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, "image.jpg");
            File.WriteAllText(filePath, "image content");

            // Act.
            var result = this._fileService.GetFileStreamPreview(id);

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FileStream);

            using (result.FileStream)
            {
                Assert.IsTrue(result.FileStream.Name.EndsWith("image.jpg"));
            }
        }

        [TestMethod]
        public void GetFileStreamPreview_ShouldReturnNotFoundImage_WhenFileIsNotPreviewable()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var folder = Path.Combine(this._rootPath,
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));

            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "file.txt"), "text content");
            var notFoundPath = Path.Combine(this._rootPath, "notfound.png");
            File.WriteAllText(notFoundPath, "not found image");

            // Act.
            var result = this._fileService.GetFileStreamPreview(id);

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FileStream);

            using (result.FileStream)
            {
                Assert.IsTrue(result.FileStream.Name.EndsWith("notfound.png"));
            }
        }

        [TestMethod]
        public void GetFileStreamDownload_ShouldReturnFileStreamWithCorrectNameAndMimeType_IfValidData()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var folder = Path.Combine(this._rootPath,
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));

            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, "document.pdf");
            File.WriteAllText(filePath, "pdf content");

            // Act.
            var result = this._fileService.GetFileStreamDownload(id);

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FileStream);
            Assert.AreEqual("document.pdf", result.FileName);
            Assert.AreEqual("application/pdf", result.MimeType);

            result.FileStream.Dispose();
        }

        [TestMethod]
        public void DeleteFolder_ShouldRemoveExistingFolder_WhenValidData()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var folder = Path.Combine(this._rootPath,
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));
            Directory.CreateDirectory(folder);

            // Early Assert.
            Assert.IsTrue(Directory.Exists(folder));

            // Act.
            this._fileService.DeleteFolder(id);

            // Assert.
            Assert.IsFalse(Directory.Exists(folder));
        }

        /// <summary>
        /// This logic didn't work once. The file got deleted, but the folder didnt.
        /// The test passed though. I assume this is a permissions-related quirk. 
        /// If it happens again, firstly look at this test.
        /// </summary>
        [TestMethod]
        public void DeleteFolder_ShouldRemoveFolderAndContents_WhenFolderHasFiles()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var folder = Path.Combine(this._rootPath,
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));
            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, "test.txt");
            File.WriteAllText(filePath, "Hello world");

            // Early assert.
            Assert.IsTrue(Directory.Exists(folder));
            Assert.IsTrue(File.Exists(filePath));

            // Act.
            this._fileService.DeleteFolder(id);

            // Assert.
            Assert.IsFalse(File.Exists(filePath));
            Assert.IsFalse(Directory.Exists(folder));
        }

        /// <summary>
        /// DeleteFolder calls a seperate method that finds the full path. This method auto-creates
        /// the folder, so it should always exist, even if the folder did not exist beforehand.
        /// The test is here to ensure the folder is created beforehand and does not crash things.
        /// </summary>
        [TestMethod]
        public void DeleteFolder_ShouldCreateNewFolderAndNotThrow_IfFolderDidNotExistBefore()
        {
            // Arrange.
            var id = Guid.NewGuid();
            var folder = Path.Combine(this._rootPath,
                id.ToString().Replace("-", Path.DirectorySeparatorChar.ToString()));

            // Act.
            this._fileService.DeleteFolder(id);

            // Assert.
            Assert.IsFalse(Directory.Exists(folder));
        }

        private IFormFile CreateFakeFormFile(string fileName = "test.jpg")
        {
            var content = "fake media file";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            return new FormFile(stream, 0, stream.Length, "file", fileName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this._rootPath))
                Directory.Delete(this._rootPath, recursive: true);
        }
    }
}

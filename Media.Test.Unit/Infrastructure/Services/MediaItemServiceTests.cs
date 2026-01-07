using Azure.Core;
using Media.Abstractions.Interfaces;
using Media.Core.Dtos.Exchange;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Core.Options;
using Media.Infrastructure.Services;
using Media.Persistence;
using Media.Test.Unit.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Unit.Infrastructure.Services
{
    [TestClass]
    public class MediaItemServiceTests : TestWithInMemoryDb
    {
        private AuthTokenService _authTokenService;
        private MediaItemService _mediaItemService;

        private string _testingToken = string.Empty;

        private IOptions<UploadPolicyOptions> _options;

        [TestInitialize]
        public async Task Setup()
        {
            this.BaseSetup();

            this._options = Options.Create(new UploadPolicyOptions(20, new List<string> { ".exe" }));
            this._authTokenService = new AuthTokenService(this._context);
            this._mediaItemService = new MediaItemService(
                this._authTokenService,
                this._context,
                new TestingFileService() { ShouldThrowOnUpload = false },
                this._options);

            var request = new CreateTokenRequest("TestingToken", DateTime.Now.AddDays(1));
            var result = await this._authTokenService.CreateToken(request);
            this._testingToken = result.Token;
        }

        [TestMethod]
        public async Task UploadMediaItem_ShouldCreateMediaItem_WhenAuthorized()
        {
            // Arrange.
            var request = new UploadMediaItemRequest("Testing File", null, this.CreateFakeFormFile());

            // Act.
            var response = await this._mediaItemService.UploadMediaItem(request, this._testingToken);

            // Assert.
            Assert.AreNotEqual(Guid.Empty, response.Id);

            var mediaItem = await this._context.MediaItems.FirstOrDefaultAsync();
            Assert.IsNotNull(mediaItem);
            Assert.AreEqual(this._testingToken, mediaItem.CreatedByToken);
        }

        
        [TestMethod]
        public async Task UploadMediaItem_ShouldThrowUnauthorizedException_WhenTokenLacksPermission()
        {
            // Arrange.
            var mediaRequest = new UploadMediaItemRequest("Testing File", null, this.CreateFakeFormFile());
            var tokenRequest = new CreateTokenRequest("ReadonlyToken", DateTime.Now.AddDays(1));
            var tokenResult = await this._authTokenService.CreateToken(tokenRequest);
            
            var token = await this._context.AuthTokens.SingleOrDefaultAsync(t => t.Token == tokenResult.Token);
            token!.Permissions = AuthTokenPermissions.CanRead;
            await this._context.SaveChangesAsync();
            
            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await this._mediaItemService.UploadMediaItem(mediaRequest, tokenResult.Token);
            });

            // Assert.
            Assert.AreEqual("Could not upload this media item. Provided token does not have the CanCreate permission.", ex.Message);
        }

        [TestMethod]
        public async Task UploadMediaItem_ShouldNotSaveDb_WhenFileFails()
        {
            // Arrange.
            var request = new UploadMediaItemRequest("Testing File", null, this.CreateFakeFormFile());
            var service = new MediaItemService(
                this._authTokenService, this._context, new TestingFileService() { ShouldThrowOnUpload = true }, this._options);

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<Exception>(async () =>
                await service.UploadMediaItem(request, this._testingToken)
            );

            // Assert.
            Assert.AreEqual(0, await this._context.MediaItems.CountAsync());
            Assert.AreEqual("Could not upload the file on the testing file service.", ex.Message);
        }


        [TestMethod]
        public async Task GetMediaItemFileStreamPreview_ShouldReturnFile_WhenMediaItemExists()
        {
            // Arrange.
            var mediaRequest = new UploadMediaItemRequest("Testing File", null, this.CreateFakeFormFile());
            var response = await this._mediaItemService.UploadMediaItem(mediaRequest, this._testingToken);

            // Act.
            var result = await this._mediaItemService.GetMediaItemFileStreamPreview(response.Id);

            // Assert.
            using (result.FileStream)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.FileStream.Name.EndsWith("test.jpg"));
            }
        }

        [TestMethod]
        public async Task GetMediaItemFileStreamPreview_ShouldThrowNotFoundException_WhenMissing()
        {
            // Arrange.
            var missingId = Guid.NewGuid();

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
                await this._mediaItemService.GetMediaItemFileStreamPreview(missingId)
            );

            // Assert.
            Assert.AreEqual("Media Item is not found", ex.Message);
        }

        [TestMethod]
        public async Task GetMediaItemFileStreamDownload_ShouldReturnDownload_WhenMediaItemExists()
        {
            // Arrange.
            var mediaRequest = new UploadMediaItemRequest("Testing File", null, this.CreateFakeFormFile());
            var response = await this._mediaItemService.UploadMediaItem(mediaRequest, this._testingToken);

            // Act.
            var result = await this._mediaItemService.GetMediaItemFileStreamDownload(response.Id);

            // Assert.
            using (result.FileStream)
            {
                Assert.IsNotNull(result);
                Assert.IsTrue(result.FileStream.Name.EndsWith("test.jpg"));
            }
        }

        [TestMethod]
        public async Task GetMediaItemFileStreamDownload_ShouldThrowNotFoundException_WhenMissing()
        {
            // Arrange.
            var missingId = Guid.NewGuid();

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
                await this._mediaItemService.GetMediaItemFileStreamDownload(missingId)
            );

            // Assert.
            Assert.AreEqual("Media Item is not found", ex.Message);
        }

        [TestMethod]
        public async Task GetInfo_ShouldReturnCorrectInfo_WhenMediaItemExists()
        {
            // Arrange.
            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid(),
                CreatedByToken = this._testingToken,
                Title = "Test Title",
                Description = "Test Description"
            };
            this._context.MediaItems.Add(mediaItem);
            await this._context.SaveChangesAsync();

            // Act.
            var result = await this._mediaItemService.GetInfo(mediaItem.Id);

            // Assert.
            Assert.IsNotNull(result);
            Assert.AreEqual(this._testingToken, result.CreatedBy);
            Assert.AreEqual("Test Title", result.Title);
            Assert.AreEqual("Test Description", result.Description);
        }

        [TestMethod]
        public async Task GetInfo_ShouldThrowNotFoundException_WhenMediaItemDoesNotExist()
        {
            // Arrange.
            var missingId = Guid.NewGuid();

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                this._mediaItemService.GetInfo(missingId)
            );
            
            // Assert.
            Assert.AreEqual("Media Item is not found", ex.Message);
        }

        [TestMethod]
        public async Task ByToken_ShouldReturnPaginatedItems_ForGivenToken()
        {
            // Arrange.
            var items = new[]
            {
                new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "A"},
                new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "B"},
                new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "C"},
                new MediaItem { Id = Guid.NewGuid(), CreatedByToken = "OtherToken", Title = "Other"}
            };

            this._context.MediaItems.AddRange(items);
            await this._context.SaveChangesAsync();

            var pagination = new PaginationReq { PageNumber = 1, PageSize = 2 };

            // Act.
            var result = await this._mediaItemService.ByToken(this._testingToken, pagination);

            // Assert.
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual("A", result.Items[0].Title);
            Assert.AreEqual("B", result.Items[1].Title);

            Assert.AreEqual(3, result.Pagination.TotalItems);
            Assert.AreEqual(2, result.Pagination.PageSize);
            Assert.AreEqual(1, result.Pagination.PageNumber);
            Assert.AreEqual(2, result.Pagination.TotalPages);
        }

        [TestMethod]
        public async Task DeleteById_ShouldRemoveItemAndDeleteFolder_WhenAuthorized()
        {
            // Arrange.
            var item = new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "Test" };
            this._context.MediaItems.Add(item);
            await this._context.SaveChangesAsync();

            // Act.
            await this._mediaItemService.DeleteById(item.Id, this._testingToken);

            // Assert.
            var exists = await this._context.MediaItems.AnyAsync(m => m.Id == item.Id);
            Assert.IsFalse(exists);
        }


        [TestMethod]
        public async Task DeleteById_ShouldThrowNotFoundException_WhenItemMissing()
        {
            // Arrange.

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
                this._mediaItemService.DeleteById(Guid.NewGuid(), this._testingToken)
            );

            // Assert.
            Assert.AreEqual("Media Item is not found", ex.Message);
        }

        [TestMethod]
        public async Task DeleteById_ShouldThrowUnauthorizedException_WhenTokenCannotDelete()
        {
            // Arrange.
            var item = new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "Test" };
            this._context.MediaItems.Add(item);

            var tokenRequest = new CreateTokenRequest("ReadonlyToken", DateTime.Now.AddDays(1));
            var tokenResult = await this._authTokenService.CreateToken(tokenRequest);

            var token = await this._context.AuthTokens.SingleOrDefaultAsync(t => t.Token == tokenResult.Token);
            token!.Permissions = AuthTokenPermissions.CanRead;
            await this._context.SaveChangesAsync();

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(() =>
                this._mediaItemService.DeleteById(item.Id, tokenResult.Token)
            );

            // Assert.
            Assert.IsTrue(ex.Message.Contains("Could not delete this media item"));
        }

        [TestMethod]
        public async Task DeleteById_ShouldThrowUnauthorizedException_WhenTokenDoesNotOwnItem()
        {
            // Arrange.
            var item = new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "Test" };
            this._context.MediaItems.Add(item);
            await this._context.SaveChangesAsync();

            var tokenRequest = new CreateTokenRequest("ReadonlyToken", DateTime.Now.AddDays(1));
            var tokenResult = await this._authTokenService.CreateToken(tokenRequest);

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(() =>
                this._mediaItemService.DeleteById(item.Id, tokenResult.Token)
            );

            // Assert.
            Assert.IsTrue(ex.Message.Contains("does not own media item"));
        }

        [TestMethod]
        public async Task ModifyById_ShouldUpdateTitleAndDescription_WhenAuthorized()
        {
            // Arrange.
            var item = new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "Titl", Description = "Desc" };
            this._context.MediaItems.Add(item);
            await this._context.SaveChangesAsync();
            var updateReq = new ModifyMediaItemRequest("New Title", "New Desc");

            // Act.
            await this._mediaItemService.ModifyById(item.Id, this._testingToken, updateReq);

            // Assert.
            var updated = await this._context.MediaItems.SingleAsync(m => m.Id == item.Id);
            Assert.AreEqual("New Title", updated.Title);
            Assert.AreEqual("New Desc", updated.Description);
        }

        [TestMethod]
        public async Task ModifyById_ShouldThrowNotFoundException_WhenItemMissing()
        {
            // Arrange.
            var updateReq = new ModifyMediaItemRequest("New Title", "New Desc");

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
                await this._mediaItemService.ModifyById(Guid.NewGuid(), this._testingToken, updateReq)
            );

            // Assert.
            Assert.AreEqual("Media Item is not found", ex.Message);
        }

        [TestMethod]
        public async Task ModifyById_ShouldThrowUnauthorizedException_WhenTokenCannotModify()
        {
            // Arrange.
            var item = new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "Titl", Description = "Desc" };
            this._context.MediaItems.Add(item);
            await this._context.SaveChangesAsync();

            var tokenResult = await this._authTokenService.CreateToken(new CreateTokenRequest("FakeToken", DateTime.Now.AddDays(1)));
            var token = await this._context.AuthTokens.SingleOrDefaultAsync(t => t.Token == tokenResult.Token);
            token!.Permissions = AuthTokenPermissions.CanRead;
            await this._context.SaveChangesAsync();
            var updateReq = new ModifyMediaItemRequest("New Title", "New Desc");

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
                await this._mediaItemService.ModifyById(item.Id, tokenResult.Token, updateReq)
            );

            // Assert.
            Assert.AreEqual("Could not modify this media item. Provided token does not have the CanModify permission.", ex.Message);

        }

        [TestMethod]
        public async Task ModifyById_ShouldThrowUnauthorizedException_WhenTokenDoesNotOwnItem()
        {
            // Arrange.
            var item = new MediaItem { Id = Guid.NewGuid(), CreatedByToken = this._testingToken, Title = "Titl", Description = "Desc" };
            this._context.MediaItems.Add(item);
            await this._context.SaveChangesAsync();
            var fakeToken = await this._authTokenService.CreateToken(new CreateTokenRequest("FakeToken", DateTime.Now.AddDays(1)));

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
                await this._mediaItemService.ModifyById(item.Id, fakeToken.Token, new ModifyMediaItemRequest("New Title", "New Desc"))
            );

            Assert.AreEqual("Could not modify this media item. Provided token does not own media item.", ex.Message);
        }


        [TestMethod]
        public void CheckFormFileValid_ShouldPass_ForValidFile()
        {
            // Arrange.
            var file = this.CreateFakeFormFile(content: "ABC");
            
            // Act.
            this._mediaItemService.CheckFormFileValid(file);
            
            // Assert.
        }

        [TestMethod]
        public void CheckFormFileValid_ShouldThrow_ForBlockedExtension()
        {
            // Arrange.
            var file = this.CreateFakeFormFile(fileName: "malware.exe");

            // Act.
            var ex = Assert.ThrowsException<BadRequestException>(() =>
                this._mediaItemService.CheckFormFileValid(file)
            );

            // Assert.
            Assert.AreEqual("Files of type '.exe' are not allowed.", ex.Message);
        }

        [TestMethod]
        public void CheckFormFileValid_ShouldThrow_ForTooLargeFile()
        {
            // Arrange.
            var file = CreateFakeFormFile(content: "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            // Act.
            var ex = Assert.ThrowsException<BadRequestException>(() =>
                this._mediaItemService.CheckFormFileValid(file)
            );

            // Assert.
            Assert.AreEqual($"File is too large. Limit is {this._options.Value.MaxFileSize} bytes.", ex.Message);
        }

        private IFormFile CreateFakeFormFile(string? content = null, string fileName = "test.jpg")
        {
            if (content == null)
                content = "fake media file";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, "file", fileName);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var path = Path.Combine(Path.GetTempPath(), "media-api-tests");
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
    }
}

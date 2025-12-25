using Azure.Core;
using Media.Abstractions.Interfaces;
using Media.Core.Dtos.Exchange;
using Media.Core.Entities;
using Media.Core.Exceptions;
using Media.Infrastructure.Services;
using Media.Persistence;
using Media.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Test.Infrastructure.Services
{
    [TestClass]
    public class AuthTokenServiceTests : TestWithInMemoryDb
    {
        private IAuthTokenService _service;

        [TestInitialize]
        public void Setup()
        {
            this.BaseSetup();
            this._service = new AuthTokenService(this._context);
        }

        [TestMethod]
        public async Task CreateToken_ShouldCreateToken_WhenValidData()
        {
            // Arrange.
            var request = new CreateTokenRequest("MyToken", DateTime.Now.AddDays(1));

            // Act.
            var result = await this._service.CreateToken(request);

            // Assert.
            Assert.IsNotNull(result.Token);

            var tokenInDb = await this._context.AuthTokens.FirstOrDefaultAsync(token => token.Name == "MyToken");
            Assert.IsNotNull(tokenInDb);
            Assert.IsTrue(tokenInDb.IsActive);
        }

        [TestMethod]
        public async Task CreateToken_ShouldThrowAlreadyUsedException_WhenNameAlreadyUsed()
        {
            // Arrange.
            var request = new CreateTokenRequest("MyDupe", DateTime.Now.AddDays(1));

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<AlreadyUsedException>(async () =>
            {
                await this._service.CreateToken(request);
                await this._service.CreateToken(request);
            });

            // Assert.
            Assert.AreEqual("Token name 'MyDupe' is already being used", ex.Message);
        }


        [TestMethod]
        public async Task FindTokenInfo_ShouldGiveTokenInfo_WhenValidData()
        {
            // Arrange.
            var request = new CreateTokenRequest("InfoToken", DateTime.Now.AddDays(1));
            var result = await this._service.CreateToken(request);

            // Act.
            var info = await this._service.FindTokenInfo(result.Token);

            // Assert.
            Assert.IsTrue(info.IsActive);
            Assert.AreEqual("InfoToken", info.Name);
        }


        [TestMethod]
        public async Task FindTokenInfo_ShouldThrowNotFoundException_WhenInvalidToken()
        {
            // Arrange.

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                var info = await this._service.FindTokenInfo("InvalidToken");
            });

            // Assert.
            Assert.AreEqual("Token 'InvalidToken' does not exist", ex.Message);
        }

        [TestMethod]
        public async Task DeactivateToken_ShouldDeactivateToken_WhenValidData()
        {
            // Arrange.
            var request = new CreateTokenRequest("ActiveToken", DateTime.Now.AddDays(1));
            var result = await this._service.CreateToken(request);

            // Act.
            await this._service.DeactivateToken(result.Token);

            // Assert.
            var tokenInfo = await this._service.FindTokenInfo(result.Token);
            Assert.IsFalse(tokenInfo.IsActive);
        }

        [TestMethod]
        public async Task DeactivateToken_ShouldThrowNotFoundException_WhenInvalidToken()
        {
            // Arrange.

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await this._service.DeactivateToken("InvalidToken");
            });

            // Assert.
            Assert.AreEqual("Token 'InvalidToken' does not exist", ex.Message);
        }

        [TestMethod]
        public async Task GetRoles_ShouldReturnRoles_WhenValidData()
        {
            // Arrange.
            var request = new CreateTokenRequest("RoleToken", DateTime.Now.AddDays(1));
            var result = await this._service.CreateToken(request);

            // Act.
            var roles = await this._service.GetRoles(result.Token);

            // Assert.
            Assert.AreEqual((AuthTokenPermissions)15, roles);
        }

        [TestMethod]
        public async Task GetRoles_ShouldThrowNotFoundException_WhenInvalidToken()
        {
            // Arrange.

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await this._service.DeactivateToken("InvalidToken");
            });

            // Assert.
            Assert.AreEqual("Token 'InvalidToken' does not exist", ex.Message);
        }

        [TestMethod]
        public async Task ChangePermissions_ShouldUpdatePermissions_WhenAuthorized()
        {
            // Arrange.
            var managerTokenReq = new CreateTokenRequest("ManagerToken", DateTime.Now.AddDays(1));
            var managerResult = await this._service.CreateToken(managerTokenReq);
          
            var managerEntity = await this._context.AuthTokens.FirstAsync(t => t.Token == managerResult.Token);
            managerEntity.Permissions = AuthTokenPermissions.CanManagePermissions;
            this._context.AuthTokens.Update(managerEntity);
            await this._context.SaveChangesAsync();

            var targetTokenReq = new CreateTokenRequest("TargetToken", DateTime.Now.AddDays(1));
            var targetResult = await this._service.CreateToken(targetTokenReq);

            var request = new ChangeTokenPermissionRequest(targetResult.Token, AuthTokenPermissions.CanModify);

            // Act.
            await this._service.ChangePermissions(request, managerResult.Token);

            // Assert.
            var updated = await this._context.AuthTokens.FirstAsync(t => t.Token == targetResult.Token);
            Assert.AreEqual(AuthTokenPermissions.CanModify, updated.Permissions);
        }

        [TestMethod]
        public async Task ChangePermissions_ShouldThrowUnauthorizedException_WhenTokenNotAllowed()
        {
            // Arrange.
            var managerTokenReq = new CreateTokenRequest("ManagerToken", DateTime.Now.AddDays(1));
            var managerResult = await this._service.CreateToken(managerTokenReq);
          
            var targetTokenReq = new CreateTokenRequest("TargetToken", DateTime.Now.AddDays(1));
            var targetResult = await this._service.CreateToken(targetTokenReq);

            var request = new ChangeTokenPermissionRequest(targetResult.Token, AuthTokenPermissions.CanModify);

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(async () =>
            {
                await this._service.ChangePermissions(request, targetResult.Token);
            });

            // Assert.
            Assert.AreEqual("Provided token does not have the needed permissions.", ex.Message);
        }

        [TestMethod]
        public async Task ChangePermissions_ShouldThrowNotFoundException_WhenTargetTokenMissing()
        {
            // Arrange.
            var managerTokenReq = new CreateTokenRequest("ManagerToken", DateTime.Now.AddDays(1));
            var managerResult = await this._service.CreateToken(managerTokenReq);
                
            var managerEntity = await this._context.AuthTokens.FirstAsync(t => t.Token == managerResult.Token);
            managerEntity.Permissions = AuthTokenPermissions.CanManagePermissions;
            this._context.AuthTokens.Update(managerEntity);
            await this._context.SaveChangesAsync();

            var request = new ChangeTokenPermissionRequest("FakeToken", AuthTokenPermissions.CanModify);

            // Act.
            var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await this._service.ChangePermissions(request, managerResult.Token);
            });

            // Assert.
            Assert.AreEqual("Token 'FakeToken' does not exist", ex.Message);
        }
    }
}

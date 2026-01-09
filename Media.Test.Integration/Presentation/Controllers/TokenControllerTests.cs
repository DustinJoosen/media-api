using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Media.Abstractions.Interfaces;
using Media.Core.Dtos.Exchange;
using Media.Core.Entities;
using Media.Persistence;
using Media.Presentation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Media.Test.Integration.Presentation.Controllers
{
    [TestClass]
    public class TokenControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private string _token;

        [TestInitialize]
        public async Task Setup()
        {
            // Override the database, use a memory db instead.
            this._factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove and then replace the database.
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<MediaDbContext>));

                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<MediaDbContext>(options =>
                            options.UseInMemoryDatabase("TokenControllerTests"));
                    });
                });

            this._client = this._factory.CreateClient();

            // Default token.
            this._token = "ABCDEFGHIJKLMNOP";
            try
            {
                var scope = this._factory.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<MediaDbContext>();

                var authToken = new AuthToken { Name = "Test Token", Token = this._token, IsActive = true, Permissions = AuthTokenPermissions.CanManagePermissions };
                dbContext.AuthTokens.Add(authToken);
                await dbContext.SaveChangesAsync();
            } catch { }

            // Add token to default authorization header.
            this._client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", this._token);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this._client?.Dispose();
            this._factory?.Dispose();
        }

        [TestMethod]
        public async Task POST_CreateToken_ReturnsCreatedToken()
        {
            // Arrange.
            var request = new CreateTokenRequest("Creation Test Token", null);

            // Act.
            var response = await this._client.PostAsJsonAsync("/tokens/create-token", request);

            // Assert.
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var tokenResponse = await response.Content.ReadFromJsonAsync<CreateTokenResponse>();

            Assert.IsNotNull(tokenResponse);
            Assert.AreNotEqual(0, tokenResponse.Token.Length);
        }

        [TestMethod]
        public async Task GET_FindTokenInfo_ReturnsTokenInfo()
        {
            // Arrange.
            var request = new HttpRequestMessage(HttpMethod.Get, "/tokens/info");

            // Act.
            var response = await this._client.SendAsync(request);
            
            // Assert.
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var info = await response.Content.ReadFromJsonAsync<FindTokenInfoResponse>();
            
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public async Task PUT_ChangePermissions_ReturnsOk()
        {
            // Arrange.
            var payload = new ChangeTokenPermissionRequest(this._token, AuthTokenPermissions.CanManagePermissions);
            var request = new HttpRequestMessage(HttpMethod.Put, "/tokens/change-permissions")
            {
                Content = JsonContent.Create(payload)
            };

            // Act.
            var response = await this._client.SendAsync(request);
            
            // Assert.
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual($"Token '{this._token}' has been given new permissions.", content);
        }

        [TestMethod]
        public async Task DELETE_DeactivateToken_ReturnsOk()
        {
            // Arrange.
            var request = new HttpRequestMessage(HttpMethod.Delete, "/tokens/deactivate-token");

            // Act.
            var response = await this._client.SendAsync(request);

            // Assert.
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual($"Token '{this._token}' successfully deactivated.", content);

            // Clean.
            var cleanReq = new CreateTokenRequest("Creation Test Token", null);
            var cleanRes = await this._client.PostAsJsonAsync("/tokens/create-token", request);
            this._token = (await cleanRes.Content.ReadFromJsonAsync<CreateTokenResponse>())?.Token ?? this._token;
        }
    }
}

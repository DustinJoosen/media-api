using Media.Core.Dtos.Exchange;
using Media.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Media.Presentation;

namespace Media.Test.Integration.Presentation.Controllers
{
    [TestClass]
    public class HealthControllerTests
    {
        private static WebApplicationFactory<Program> _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            HealthController.AppRunningTime = DateTime.Now;

            HealthControllerTests._factory = new WebApplicationFactory<Program>();
            HealthControllerTests._client = HealthControllerTests._factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            HealthControllerTests._client?.Dispose();
            HealthControllerTests._factory?.Dispose();
        }

        [TestMethod]
        public async Task GET_Health_ReturnsOkHealth()
        {
            // Arrange.

            // Act.
            var response = await _client.GetAsync("/health");

            // Assert.
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var health = await response.Content.ReadFromJsonAsync<HealthResponse>();
            
            Assert.IsNotNull(health);
            Assert.AreEqual("Ok", health.Status);
            Assert.IsNotNull(health.Runtime);
            Assert.IsNotNull(health.Dependencies);
        }

    }
}

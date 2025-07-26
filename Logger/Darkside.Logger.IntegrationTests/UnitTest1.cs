using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Darkside.Logging.Logger.API;

namespace Darkside.Logger.IntegrationTests
{
    public class LoggerApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LoggerApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddLog_ShouldReturnSuccess()
        {
            // Arrange
            var requestContent = new StringContent("{\"TenantId\":\"123\",\"Application\":\"TestApp\",\"LogLevel\":\"Info\",\"Message\":\"Test log message\"}", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/AddLog", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RetrieveLog_ShouldReturnLogs()
        {
            // Arrange
            var requestContent = new StringContent("{\"TenantId\":\"123\",\"Application\":\"TestApp\"}", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/RetrieveLog", requestContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNullOrEmpty();
        }
    }
}

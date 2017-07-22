using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DeviceCache.Tests
{
    public class AcceptanceTests
    {
        private readonly HttpClient _httpClient;
        
        public AcceptanceTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("test-settings.json");

            var configuration = builder.Build();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["frontendEndpoint"])
            };
        }

        [Fact]
        public async Task ShouldReadCachedDataSentFromDevice()
        {
            // Act
            var response = await GetDataFromCache();

            // Assert
            response.Should().Contain("Works");
        }

        [Fact]
        public async Task ShouldNotFindNeverSentDataInTheCache()
        {
            // Act
            var response = await GetDataFromCache();

            // Assert
            response.Should().NotContain(Guid.NewGuid().ToString());
        }

        private async Task<string> GetDataFromCache()
        {
            var response = await _httpClient.GetAsync("api/data");
            var responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
    }
}
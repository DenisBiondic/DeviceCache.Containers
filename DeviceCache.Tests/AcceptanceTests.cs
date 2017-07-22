using System;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DeviceCache.Tests
{
    public class AcceptanceTests
    {
        public AcceptanceTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("test-settings.json");

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        [Fact]
        public void ShouldReadCachedDataSentFromDevice()
        {
            // Arrange

            // Act
            throw new NotImplementedException();

            // Assert
        }
    }
}
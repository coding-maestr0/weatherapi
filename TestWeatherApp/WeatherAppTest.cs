using DataModels;
using DataServiceRepository;
using DataServiceRepository.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;

namespace DataServiceRepository.Tests
{
    public class WeatherForecastServiceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public WeatherForecastServiceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetWeatherForecast_WithValidLocation_ReturnsWeatherEntity()
        {
            // Arrange
            var location = "New York";
            var requestUri = $"api/WeatherForecast/getWeatherData?loc={location}";
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            // Assert
            response.EnsureSuccessStatusCode();

        }
    }
}

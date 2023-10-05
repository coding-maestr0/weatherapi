using DataModels;
using DataServiceRepository.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace DataServiceRepository
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IConfiguration _configuration;
        private string currentLocationWeather = Helpers.Helpers.current;
        private readonly IHttpClientFactory _httpClientFactory;
        private ILogger<WeatherForecastService> _logger;
        private readonly IMemoryCacheService<WeatherEntity> _cache;

        public WeatherForecastService(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,
            IMemoryCacheService<WeatherEntity> cache,
            ILogger<WeatherForecastService> logger
            )
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        public async Task<(bool, WeatherEntity)> GetWeatherForecast(string location)
        {
            WeatherEntity weatherEntity = new WeatherEntity();
            if (string.IsNullOrEmpty(location))
            {
                weatherEntity.error.message = "Something went wrong!";
                return (false, weatherEntity);
            }
            
            bool status = false;
            try
            {
                weatherEntity = _cache.GetValueFromCache(location);
                if (weatherEntity != null)
                {
                    _logger.Log(LogLevel.Information, "Weather forecast found in cache.");
                    status = true;
                }
                else 
                {
                    var httpClient = _httpClientFactory.CreateClient("weatherClient");
                    using (var response = await httpClient.GetAsync(currentLocationWeather + "?key=" + GetAPIKey() + "&q=" + location))
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        weatherEntity = JsonConvert.DeserializeObject<WeatherEntity>(json)!;
                        _cache.SetValueInCache(location, weatherEntity);
                        status = response.IsSuccessStatusCode ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
                weatherEntity.error.message = "Something went wrong!";
            }

            return (status, weatherEntity);
        }

        private string GetAPIKey() => _configuration["WeatherForecastConfigs:apikey"]!;
       
    }
}

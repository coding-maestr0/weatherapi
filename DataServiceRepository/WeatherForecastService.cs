using DataModels;
using DataServiceRepository.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace DataServiceRepository
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IConfiguration _configuration;
        private string currentLocationWeather = Helpers.Helpers.CURRENT;
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

        public async Task<(bool, string, WeatherEntity)> GetWeatherForecast(string location)
        {
            bool status = false;
            string message = string.Empty;
            WeatherEntity weatherEntity = new WeatherEntity();
            if (string.IsNullOrEmpty(location))
            {
                message = Helpers.Helpers.PROVIDELOCATION;
                return (false, message, weatherEntity);
            }
            try
            {
                weatherEntity = _cache.GetValueFromCache(location);
                if (weatherEntity != null)
                {
                    status = weatherEntity.error != null ? false : true;
                    _logger.Log(LogLevel.Information, Helpers.Helpers.DATAFOUNDINCACHE);
                }
                else
                {
                    var httpClient = _httpClientFactory.CreateClient("weatherClient");
                    using (var response = await httpClient.GetAsync(currentLocationWeather + "?key=" + GetAPIKey() + "&q=" + location + "&&days=7"))
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                                weatherEntity = JsonConvert.DeserializeObject<WeatherEntity>(json)!;
                                _cache.SetValueInCache(location, weatherEntity);
                                status = true;
                                message = "success";
                                break;
                            case HttpStatusCode.BadRequest:
                                status = false;
                                var deserialize = JsonConvert.DeserializeObject<WeatherEntity>(json)!;
                                message = deserialize.error.message;

                                break;
                            default:
                                status = false;
                                message = Helpers.Helpers.SOMETHINGWENTWRONG;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
                message = Helpers.Helpers.SOMETHINGWENTWRONG;
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return (status, message, weatherEntity);
        }



        private string GetAPIKey() => _configuration["WeatherForecastConfigs:apikey"]!;

    }
}

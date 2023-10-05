using DataServiceRepository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        public WeatherForecastController(IWeatherForecastService weatherForecastService) 
        { 
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet("getWeatherData")]
        public async Task<IActionResult> GetWeatherData(string loc)
        {
            try
            {
                var getCurretnWeather = await _weatherForecastService.GetWeatherForecast(loc);
                return Ok(new { status = getCurretnWeather.Item1, data = getCurretnWeather.Item2 });
            }
            catch(Exception ex)
            {
                return BadRequest(new { status = false, data = ex.Message });
            }
        }
    }
}
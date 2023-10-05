using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceRepository.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<(bool, WeatherEntity)> GetWeatherForecast(string input);
    }
}

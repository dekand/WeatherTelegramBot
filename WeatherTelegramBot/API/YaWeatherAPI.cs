using Newtonsoft.Json;
using System.Drawing;
using System;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot.API
{
    internal class YaWeatherAPI
    {
        /// <summary>
        /// Determines weather parameters for an instance of a <seealso cref="CityDate"/> object (by longitude and latitude).
        /// </summary>
        /// <param name="geo"><seealso cref="CityDate"/> object instance.</param>
        /// <param name="apiKey">Weather API key.</param>
        /// <returns><seealso cref="YaWeather"/> object instance.</returns>
        public static async Task<YaWeather> GetWeather(CityDate geo, string apiKey)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Yandex-API-Key", apiKey);
            string response = await httpClient.GetStringAsync($"https://api.weather.yandex.ru/v2/fact?lat={geo.Latitude.ToString().Replace(',', '.')}&lon={geo.Longitude.ToString().Replace(',', '.')}");
            await Console.Out.WriteLineAsync("\n"+response+"\n");
            var responseWeather = JsonConvert.DeserializeObject<YaWeather>(response);
            if (responseWeather == null) { return null!; }
            return new YaWeather(
                temp: responseWeather.Temp,
                feels_Like: responseWeather.Feels_Like,
                icon: responseWeather.Icon,
                condition: responseWeather.Condition,
                wind_Speed: responseWeather.Wind_Speed,
                dayTime: responseWeather.DayTime);
        }
    }
}

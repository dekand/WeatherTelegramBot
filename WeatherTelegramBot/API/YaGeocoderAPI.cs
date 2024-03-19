using Newtonsoft.Json;
using System.Globalization;
using WeatherTelegramBot.Models;

namespace WeatherTelegramBot.API
{
    internal class YaGeocoderAPI
    {
        /// <summary>
        /// Determines coordinates (longitude, latitude) by <seealso cref="string"/> city name.
        /// </summary>
        /// <param name="messageText">City name.</param>
        /// <param name="apiKey">Geocoder API key.</param>
        /// <returns><seealso cref="CityDate"/> object instance.</returns>
        public static async Task<CityDate> GetLocation(string cityName, string apiKey)
        {
            using var httpClient = new HttpClient();
            string responseString = await httpClient.GetStringAsync($"https://geocode-maps.yandex.ru/1.x/?apikey={apiKey}&geocode={cityName}&format=json");

            var responseGeocoder = JsonConvert.DeserializeObject<YaGeocoder>(responseString);
            string? Name=null, pos=null;
            try
            {
                Name = responseGeocoder?.Response?.GeoObjectCollection?.FeatureMember?[0]?.GeoObject?.Name;
                pos = responseGeocoder?.Response?.GeoObjectCollection?.FeatureMember?[0]?.GeoObject?.Point?.pos;
            }
            catch (Exception e) { await Console.Out.WriteLineAsync(e.Message); }

            string[] coordinates = pos?.Split(' ') ?? Array.Empty<string>();

            if (coordinates.Length != 2
                || !double.TryParse(coordinates[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude)
                || !double.TryParse(coordinates[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude))
            {
                return null!;
            }
            return new CityDate(Name, longitude, latitude);
        }
    }
}

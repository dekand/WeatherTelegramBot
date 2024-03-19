namespace WeatherTelegramBot.Models
{
    internal class CityDate
    {
        public string Name { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public CityDate(string name, double longitude, double latitude)
        {
            Name = name;
            Longitude = longitude;
            Latitude = latitude;
        }

        public string GetDisplay() => $"{Name}";
    }
}

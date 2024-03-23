namespace WeatherTelegramBot.Models
{
    internal class CityData
    {
        public string Name { get; set; } = null!;
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public CityData(string name, double longitude, double latitude)
        {
            Name = name;
            Longitude = longitude;
            Latitude = latitude;
        }

        public string GetDisplay() => $"{Name}";
    }
}

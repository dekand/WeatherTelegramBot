namespace WeatherTelegramBot.Models
{
    internal class YaWeather
    {
        public int Temp;
        public int Feels_Like;
        public string Icon;
        public string Condition;
        public double Wind_Speed;
        public char DayTime;

        public YaWeather(int temp, int feels_Like, string icon, double wind_Speed, char dayTime, string condition = "Непонятная погода")
        {
            Temp = temp;
            Feels_Like = feels_Like;
            Icon = icon;
            Wind_Speed = wind_Speed;
            DayTime = dayTime;
            Condition = condition.ToUpper() switch
            {
                "CLEAR" => "Ясно",
                "PARTLY-CLOUDY" => "Переменная облачность",
                "CLOUDY" => "Облачно",
                "OVERCAST" => "Пасмурно",
                "LIGHT-RAIN" => "Небольшой дождь",
                "RAIN" => "Дождь",
                "HEAVY-RAIN" => "Ливень",
                "SHOWERS" => "Обильные осадки",
                "SLEET" => "Мокрый снег",
                "LIGHT-SNOW" => "Небольшой снег",
                "SNOW" => "Снег",
                "SNOWFALL" => "Снегопад",
                "HAIL" => "Град",
                "THUNDERSTORM" => "Гроза",
                "THUNDERSTORM-WITH-RAIN" => "Гроза с дождем",
                "THUNDERSTORM-WITH-HAIL" => "Гроза с градом",
                _ => condition,
            };
        }
        public string DisplayTemp() => $"{Temp}°";
        public string DisplayWeatherInfo() => $"Ощущается: {Feels_Like}°\nВетер: {Wind_Speed} м/с\n";
    }
}

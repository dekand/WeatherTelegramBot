# WeatherTelegramBot
Бот написан на C# с использованием .NET-клиента для Telegram Bot API [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot). Погода определяется через `API Яндекс.Погоды`, а в качестве инструмента для определения локации выступает `JavaScript API и HTTP Геокодер` тоже от Яндекса.
## На что способен?
По названию города на русском или английском (другие языки не пробовал) определяет погоду и возвращает результат в виде изображения. Изображение включает название города, текущую информацию о погоде (температуру, состояние, ощущаемая темп., скорость ветра) и [иконка состояния погоды](https://yandex.ru/dev/weather/doc/ru/concepts/icons). Создается картинка благодаря `System.Drawing`.</br>
Скриншоты:</br>
<img src="https://github.com/dekand/WeatherTelegramBot/blob/master/Demo/Screenshot_1.jpg" width="400" align="top" />
<img src="https://github.com/dekand/WeatherTelegramBot/blob/master/Demo/Screenshot_2.jpg" width="400" align="top" /></br>
Яндекс позволяет делать небольшие ошибки в названии города или сокращения (пример на 2 скриншоте). Если на введенный текст у Яндекса нет предположений, то выводится информация об ошибке (3 скрин).</br>
<img src="https://github.com/dekand/WeatherTelegramBot/blob/master/Demo/Screenshot_3.jpg" width="400" align="top" />
## Ключи API и ссылки
Для работы программы нужно создать ключи API для `Telegram бота`, `API Яндекс.Погоды`, и `JavaScript API и HTTP Геокодер`. С этой целью создан класс `APIKey`:</br>
   ```c#
internal sealed class APIKey
    {
        public static readonly string Bot = "...";
        public static readonly string Weather = "...";
        public static readonly string Geocoder = "...";
    }
```
</br>
Также создан класс, который хранит путь к иконкам состояния погоды и изображению при ошибке.</br>
   
   ```c#
internal class DirPath
    {
        public static readonly string imgDir = "...\\WeatherTelegramBot\\WeatherTelegramBot\\images\\";
        public static readonly string errorImg = "...";
    }
```


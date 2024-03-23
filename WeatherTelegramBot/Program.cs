using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WeatherTelegramBot;
using WeatherTelegramBot.API;
using WeatherTelegramBot.Models;

internal class Program
{
    private static string erText = "";

    static async Task Main(string[] args)
    {
        var botClient = new TelegramBotClient(APIKey.Bot);

        CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        await Console.Out.WriteLineAsync($"Start listening for @{me.Username}");
        await Console.In.ReadLineAsync();

        // Send cancellation request to stop bot
        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message) return;
            if (message.Text is not { } messageText) return;
            erText = "";

            var chatId = message.Chat.Id;
            var chatName = message.Chat.FirstName;

            await Console.Out.WriteLineAsync($"Received a '{messageText}' message in chat {chatId} by {chatName}.");

            Stream stream = new MemoryStream();
            stream = await CreateImageStream(stream, messageText);

            //кнопки быстрого ответа (локация, нск, ук, др)
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[]{"Новосибирск","Усть-Каменогорск"},
                new KeyboardButton[]{"Москва","Другой"}
            });


            Message sentMessage = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: erText.Any() && message.Text != "/start" && message.Text != "Другой" ? InputFile.FromUri(DirPath.errorImg) : InputFile.FromStream(stream),
                caption: erText.Any() ? $"{erText}" : null,
                parseMode: ParseMode.Html,
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);

            stream.Close();
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }

    // Get City obj (longitude, latitude) by name
    private static async Task<CityData> GetCityLocation(string city)
    {
        var cityData = await YaGeocoderAPI.GetLocation(city, APIKey.Geocoder);
        if (cityData == null) { erText = "Такой город разве существует? Не могу определить."; throw new Exception("Город не найден."); }
        return cityData;
    }

    // Get Weather obj by City (lon, lat)
    private static async Task<YaWeather> GetCityWeather(CityData cityData)
    {
        var weather = await YaWeatherAPI.GetWeather(cityData, APIKey.Weather);
        if (weather == null) { erText = "У синоптиков какие-то неполадки, не могу определить погоду."; throw new Exception("Погода не определена."); }
        return weather;
    }

    // Create image and save it in stream
    private static async Task<Stream> CreateImageStream(Stream stream, string city)
    {
        if (city == "/start" || city == "Другой")
        {
            erText = "Напиши название города (ru/en), где хочешь узнать погоду.\nВот такая, например, погода в Новосибирске.";
            city = "Новосибирск";
        }
        try
        {
            var cityData = await GetCityLocation(city);
            var weather = await GetCityWeather(cityData);

            stream = ImageCreator.ImageCreation(stream, cityData.GetDisplay(), weather);
        }
        catch (Exception e) { await Console.Out.WriteLineAsync(e.Message); }
        return stream;
    }
}


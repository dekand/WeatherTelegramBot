using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WeatherTelegramBot;
using WeatherTelegramBot.API;

internal class Program
{
    private static readonly bool RequestApi = true;

    static async Task Main(string[] args)
    {
        //await ImageCreator.ImageCreation();

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

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message) return;
            // Only process text messages
            if (message.Text is not { } messageText) return;

            string erText = "";
            if (messageText == "/start") { erText = "Напиши название города (ru/en), где хочешь узнать погоду."; }
            if (messageText.Length > 30) { erText = $"\"{message.Text}\" - Слишком длинное название для города."; }

            // Get City obj (longitude, latitude) by name
            var geo = await YaGeocoderAPI.GetLocation(messageText, APIKey.Geocoder);
            if (geo == null) { erText = "Такой город разве существует? Не могу определить."; return; }

            // Get Weather obj by City (lon, lat)
            var weather = await YaWeatherAPI.GetWeather(geo, APIKey.Weather);
            if (weather == null) { erText = "У синоптиков какие-то неполадки, не могу определить погоду."; return; }

            var chatId = message.Chat.Id;
            var chatName = message.Chat.FirstName;

            await Console.Out.WriteLineAsync($"Received a '{messageText}' message in chat {chatId} by {chatName}.");

            //кнопки быстрого ответа (локация, нск, ук, др)
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[]{KeyboardButton.WithRequestLocation("Отправить мою локацию"),"Новосибирск"},
                new KeyboardButton[]{"Усть-Каменогорск","Другой"}
            });

            Stream stream = ImageCreator.ImageCreation(new MemoryStream(), geo.GetDisplay(), weather);

            Message sentMessage = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: erText.Length > 0 ? InputFile.FromUri("https://cdn.dribbble.com/users/3068189/screenshots/5860179/cloud.jpg") : InputFile.FromStream(stream),
                caption: erText.Length > 0 ? $"{erText}" : null,
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
}


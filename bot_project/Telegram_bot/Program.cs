using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Net;

List<string> action = new List<string>() { "/start", "/help", "/books", "/bestsellers", "/myBooks", "/nobelLaureats",
        "/byAuthor", "/byGenre", "/byYear", "/byCountry", "/recommendBook", "/addBook"};
var Bot = new TelegramBotClient("5147820279:AAExt0B5BgyTTFKDnUvHF_IzzXrI69au4_Y");
List<string> Messages = new List<string>() { "", "" };
List<string> AddBook = new List<string>() { };

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

Bot.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token);

var me = await Bot.GetMeAsync();
Console.ReadLine();
cts.Cancel();

static ReplyKeyboardMarkup keyboard()
{
    var replyKeyboard = new ReplyKeyboardMarkup(
        new[] {
             new [] { new KeyboardButton("/books"), new KeyboardButton("/bestsellers") },

                    new[] { new KeyboardButton("/nobelLaureats"), new KeyboardButton("/myBooks") },

                    new[] { new KeyboardButton("/help"), new KeyboardButton("/recommendBook") },

                    new[] { new KeyboardButton("/addBook") }


        });

    replyKeyboard.ResizeKeyboard = true;

    return replyKeyboard;
}

static ReplyKeyboardMarkup bookkeyboard()
{
    var replyKeyboard = new ReplyKeyboardMarkup(
       new[]{
                               new[] { new KeyboardButton("/byAuthor"),new KeyboardButton("/byGenre") },
       });
    replyKeyboard.ResizeKeyboard = true;

    return replyKeyboard;
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;

    if (message.Text is not { } messageText)
        return;

    var Id = message.Chat.Id;
    var username = message.Chat.Username;

    if (AddBook.Count >= 1)
        AddBook.Add(messageText);

    if (action.Contains(messageText))
    {
        switch (messageText)
        {
            case "/start":
                {
                    await Bot.SendTextMessageAsync(Id, $"Вітаю, {username}! \nОберіть /help щоб оримати доступні команди.\nРозпочнемо?", replyMarkup: keyboard());

                    break;
                }
            case "/help":
                {
                    await Bot.SendTextMessageAsync(Id, $"Що ж, познайомимося? Розповім, що я вмію: \n- За допомогою /books допоможу обрати книгу " +
                        $"\n   - за автором /byAuthor  \n   - за жанром /byGenre \n- За допомогою /bestsellers подивитися найкращі книги за роком " +
                        $"\n- Ознайомитися з лауреатами нобелівських премій за роками за допомогою /nobelLaureats " +
                        $"\n-  Додати книгу в список прочитаних та поставити оцінку за допомогою /addBook\n" +
                        $"\n-  За допомогою /myBooks переглянути список обраних книг" +
                        $"- А також порекомендувати книгу /recommendBook на основі прочитаних книг\nЩо цікавить?", replyMarkup: keyboard());

                    break;
                }
            case "/books":
                {
                    Messages[0] = messageText;

                    await Bot.SendTextMessageAsync(Id, $"Що запропонувати?", replyMarkup: bookkeyboard());

                    break;
                }
            case "/bestsellers":
                {
                    Messages[0] = messageText;

                    await Bot.SendTextMessageAsync(Id, $"Очікую Який жанр цікавить. Введіть, наприклад, [history].");

                    break;
                }
            case "/addBook":
                {
                    AddBook.Add(messageText);
                        await Bot.SendTextMessageAsync(Id, $"Додамо книгу в список обраних. \nДля початку введіть назву книги.");

                    break;
                }
            case "/nobelLaureats":
                {
                    Messages[0] = messageText;

                    await Bot.SendTextMessageAsync(Id, $"Очікую рік, що вас цікавить. Введіть, наприклад, [2018].");

                    break;
                }
            case "/myBooks":
                {
                    await Bot.SendTextMessageAsync(Id, Response("/myBooks", "", Id.ToString()));

                    break;
                }
            case "/byAuthor":
                {
                    Messages[0] = messageText;

                    await Bot.SendTextMessageAsync(Id, $"Який автор цікавить? Введіть, наприклад, [Tolstoy].");

                    break;
                }
            case "/byGenre":
                {
                    Messages[0] = messageText;

                    await Bot.SendTextMessageAsync(Id, $"Який жанр цікавить? Введіть, наприклад, [love].");

                    break;
                }
            case "/recommendBook":
                {
                    await Bot.SendTextMessageAsync(Id, Response(messageText, "", Id.ToString()));
                    break;
                }
        }

    }

    if (Messages[0] != "" && !action.Contains(messageText))
        Messages[1] = messageText;

    if (AddBook.Count == 2)
        await Bot.SendTextMessageAsync(Id, $"Супер! Тепер визначимось з автором.");

    if (AddBook.Count == 3)
        await Bot.SendTextMessageAsync(Id, $"Можливо, жанр?");

    if (AddBook.Count == 4)
        await Bot.SendTextMessageAsync(Id, $"Ну і на кінець - ваша оцінка! \n(Бажано - за шкалою від 1 до 10)");

    if (AddBook.Count == 5)
    {
        await Bot.SendTextMessageAsync(Id, $"Книга додана в обрані. \nПереглянути усі обрані книги можна за допомогою команди /myBooks.");
        await Bot.SendTextMessageAsync(Id, $"Щось ще подивимось?", replyMarkup: keyboard());
        GetStr($"https://localhost:7296/addToList/id={Id}/author={AddBook[2]}/genre={AddBook[3]}/title={AddBook[1]}/rate={AddBook[4]}");
        AddBook.Clear();
    }

    if (Messages[0] != Messages[1] && Messages[1] != "" && Messages[0] != "" && !action.Contains(Messages[1]))
    {
        var Action = Messages[0];
        var Change = Messages[1];
        await Bot.SendTextMessageAsync(Id, Response(Action, Change, Id.ToString()));
        await Bot.SendTextMessageAsync(Id, $"Щось ще подивимось?", replyMarkup: keyboard());
        Messages[0] = Messages[1] = "";
        return;
    }

}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    return Task.CompletedTask;
}

string Response(string action, string change, string Id)
{
    string response = "";
    string json;
    switch (action)
    {
        case "/bestsellers":
            {
                json = GetStr($"https://localhost:7296/bestsellers/genre={change}");
                var books = JObject.Parse(json)["BookList"].ToObject<List<Book>>();
                response = $"Бестселлери за жанром {change}:";
                foreach (var book in books)
                {
                    response += $"\n- Автор: {book.author}; \n      Назва: {book.title};";
                }
                break;
            }
        case "/nobelLaureats":
            {
                json = GetStr($"https://localhost:7296/nobelLaureats/year={change}");
                var book = JObject.Parse(json).ToObject<laureates>();
                response = $"Лауреати за {change} рік: \n- Автор: {book.name} {book.surname}; \nМотивація: {book.motivation}";
                break;
            }
        case "/myBooks":
            {
                json = GetStr($"https://localhost:7296/myBoks/id={Id}");
                var books = JObject.Parse(json)["BookList"].ToObject<List<myBooks>>();
                response = $"Обрані книги:";
                foreach (var book in books)
                {
                    response += $"\n- Автор: {book.Author}; \n      Назва: {book.Title}; \n      Жанр: {book.Genre};\n      Оцінка: {book.Rate};";
                }
                break;
            }
        case "/byAuthor":
            {
                json = GetStr($"https://localhost:7296/books/author={change}");
                var books = JObject.Parse(json)["BookList"].ToObject<List<BookAuthor>>();
                response = $"Ккниги автора {change}:";
                foreach (var book in books)
                {
                    response += $"\n- {book.title};";
                }
                break;
            }
        case "/byGenre":
            {
                json = GetStr($"https://localhost:7296/books/genre={change}");
                var books = JObject.Parse(json)["BookList"].ToObject<List<Book>>();
                response = $"Ккниги жанру {change}:";
                foreach (var book in books)
                {
                    response += $"\n- Автор: {book.author}; \n      Назва: {book.title};";
                }
                break;
            }
        case "/recommendBook":
            {
                json = GetStr($"https://localhost:7296/recommendBook/id={Id}");
                var book = JObject.Parse(json).ToObject<Book>();
                if (book.author != "" || book.title != "")
                    response = $"Рекомендую прочитати: \n- Автор: {book.author}; \n      Назва: {book.title};";
                else
                    response = "Ще не обрано жодної книги.";
                break;
            }
    }
    return response;
}

static string GetStr(string A)
{

    System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(A);

    request.Method = "GET";

    WebResponse response = request.GetResponse();

    Stream s = response.GetResponseStream();

    StreamReader reader = new StreamReader(s);

    string answer = reader.ReadToEnd();

    response.Close();

    return answer;
}
public class laureates
{
    public string name { get; set; }
    public string surname { get; set; }
    public string motivation { get; set; }
}
public class Book
{
    public string title { get; set; }
    public string author { get; set; }
}
public class myBooks
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Rate { get; set; } = "";
    public string Genre { get; set; } = "";
}
public class BookAuthor
{
    public string title { get; set; }
}
public class addBook
{
    public string Title { get; set; }
    public string Id { get; set; }
    public string Author { get; set; }
    public string Rate { get; set; }
    public string Genre { get; set; }
}

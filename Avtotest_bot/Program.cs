using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Avtotest_bot.Services;
using JFA.Telegram.Console;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using User = Avtotest_bot.Models.User;
using Telegram.Bot.Types.ReplyMarkups;
using Avtotest_bot.Models.Ticket;
using Avtotest_bot.Models;

var botManager = new TelegramBotManager();
var bot = botManager.Create("6268319430:AAHTZDSj3EVggRdRDZLhlQ-eiZIvTDjoOcg");

var questionService = new QuestionService(bot);
var userService = new UserService(questionService);



botManager.Start(OnUpdate);

void OnUpdate(Update update)
{
    var (messageId, chatId, name, message, isSucces) = GetMessageInfo(update);

    if (!isSucces) return;

    var user = userService.AddUser(chatId, name);

    if (update.Type == UpdateType.CallbackQuery)
    {
        if (message.StartsWith("page"))
        {
            bot.DeleteMessageAsync(user.ChatId, update.CallbackQuery!.Message!.MessageId);
            var page = Convert.ToInt32(message.Replace("page", ""));
            ShowTickets(user, page);
        }
        if(message.StartsWith("ppage"))
        {
            bot.DeleteMessageAsync(user.ChatId, update.CallbackQuery!.Message!.MessageId);
            var page = Convert.ToInt32(message.Replace("ppage", ""));
            ShowTickets(user, page);
        }
    }

    switch (user.nextMessage)
    {
        case Enum.Default: ShowMenu(user); break;
        case Enum.InMenu: SelectMenu(user, message, messageId); break;
        case Enum.Intest: CheckAnswer(user, message); break;
    }
}

void ShowMenu(User user)
{
    var buttons = new List<List<KeyboardButton>>()
    {
        new List<KeyboardButton>()
        {
            new KeyboardButton("Start Test")
        },
        new List<KeyboardButton>()
        {
            new KeyboardButton("Tickets")
        },
        new List<KeyboardButton>()
        {
            new KeyboardButton("Show Results")
        },
        new List<KeyboardButton>()
        {
            KeyboardButton.WithRequestContact("Send Contact")
        }
    };
    bot.SendTextMessageAsync(user.ChatId, "Menu", replyMarkup: new ReplyKeyboardMarkup(buttons));
    userService.UpdateUserStep(user, Enum.InMenu);
}

Tuple<long, long, string, string, bool> GetMessageInfo(Update update)
{
    if (update.Type == UpdateType.CallbackQuery)
    {
        return new Tuple<long, long, string, string, bool>(update.CallbackQuery!.Message!.MessageId, update.CallbackQuery!.From!.Id, update.CallbackQuery.From.FirstName, update.CallbackQuery.Data!, true);
    }
    if (update.Type == UpdateType.Message)
    {
        return new Tuple<long, long, string, string, bool>(update.Message!.MessageId, update.Message!.From!.Id, update.Message.From.FirstName, update.Message.Text!, true);
    }

    return new Tuple<long, long, string, string, bool>(default, default, default!, default!, false);
}
void SelectMenu(User user, string message, long messageId)
{
    switch (message)
    {
        case "Start Test": StartTest(user); break;
        case "Tickets": ShowTickets(user); break;
        case "Show Results": ShowResults(user); break;
        case "Start":
            {
                userService.UpdateUserStep(user, Enum.Intest);
                SendQuestions(user);
            }
            break;
    }

    if (message.StartsWith("start-ticket"))
    {
        var ticketIndex = Convert.ToInt32(message.Replace("start-ticket", ""));
        StartTickets(user, ticketIndex);
    }
}
void StartTickets(User user, int ticketIndex)
{
    user.CurrentTicket = user.Tickets![ticketIndex];
    user.CurrentTicket.SetDefault();
    bot.SendTextMessageAsync(user.ChatId,
        $"Ticket {user.CurrentTicket.Index + 1}\nNumber of questions : {user.CurrentTicket.QuestionsCount}",
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Start")));
}
void ShowResults(User user)
{
    var message = "Ticket results: \n";
    message += $"Tickets: {user.Tickets!.Count(t => t.IsCompleted)}\n";
    message += $"Questions: {user.Tickets!.Sum(t => t.CorrectCount)}";
    bot.SendTextMessageAsync(user.ChatId, message);
}

void StartTest(User user)
{
    user.CurrentTicket = questionService.CreateTicket();

    bot.SendTextMessageAsync(user.ChatId,
        $"Ticket {user.CurrentTicket.Index}\nNumber of questions : {user.CurrentTicket.QuestionsCount}",
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Start")));
}

void SendQuestions(User user)
{
    questionService.ShowQuestionByIndex(user.ChatId, user.CurrentTicket!.CurrentQuestionIndex);
}
void CheckAnswer(User user, string message)
{
    try
    {
        int[] data = message.Split(',').Select(int.Parse).ToArray();
        var answer = questionService.SendAnswer(data[0], data[1]);

        if (answer) user.CurrentTicket!.CorrectCount++;
        user.CurrentTicket!.CurrentQuestionIndex++;

        if (user.CurrentTicket!.IsCompleted)
        {
            bot.SendTextMessageAsync(user.ChatId,
                $"Result: {user.CurrentTicket.CorrectCount}/{user.CurrentTicket.QuestionsCount}");

            userService.UpdateUserStep(user, Enum.InMenu);

            //user.Tickets!.Add(user.CurrentTicket);
        }
        else
        {
            SendQuestions(user);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

void ShowTickets(User user, int page = 1)
{
    var count = questionService.ticketsCount / 5;
    var message = $"Tickets:\nPage: {page}/{count}";
    var buttons = new List<List<InlineKeyboardButton>>();

    for (int i = page * 5 - 5; i < page * 5; i++)
    {
        var ticket = user.Tickets![i];
        var ticketnum = $"Ticket {ticket.Index + 1}";

        if (ticket.StartIndex != ticket.CurrentQuestionIndex)
        {
            if (ticket.CorrectCount == ticket.QuestionsCount)
            {
                ticketnum += " ✅";
            }
            else
            {
                ticketnum += $" {ticket.CorrectCount}/{ticket.QuestionsCount}";
            }
        }
        buttons.Add(new List<InlineKeyboardButton>()
        {
            InlineKeyboardButton.WithCallbackData(ticketnum, $"start-ticket{ticket.Index}")
        });
    }
    buttons.Add(CreatePaginationButtons(count, page));
    bot.SendTextMessageAsync(user.ChatId, message, replyMarkup: new InlineKeyboardMarkup(buttons));
}

List<InlineKeyboardButton> CreatePaginationButtons(int count, int page = 1)
{
    var buttons = new List<InlineKeyboardButton>();

    if (page > 1)
    {

        buttons.Add(InlineKeyboardButton.WithCallbackData("<", $"page{page - 1}"));
    }


    if (count > page)
    {
        if ((count-(count-(count/5*5))) > page)
        {

            for (int i = page; i < page + 5; i++)
            {
                buttons.Add(InlineKeyboardButton.WithCallbackData($"{i}", $"ppage{page+i-1}"));
            }
        }

        buttons.Add(InlineKeyboardButton.WithCallbackData(">", $"page{page + 1}"));
    }
    return buttons;
}






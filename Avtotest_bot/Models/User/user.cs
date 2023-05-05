using Telegram.Bot.Types.ReplyMarkups;

namespace Avtotest_bot.Models
{
    class User
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public long ChatId { get; set; }
        public Enum nextMessage { get; set; }
        public TicketM? CurrentTicket { get; set; }

        public List<TicketM>? Tickets;

        public User()
        {
            Tickets = new List<TicketM>();
        }

        public List<List<InlineKeyboardButton>> Buttons()
        {
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
            foreach (var ticket in Tickets!)
            {
                var keycha = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"Ticket {ticket.Index}"),
                    InlineKeyboardButton.WithCallbackData($"{ticket.CorrectCount}/{ticket.QuestionsCount}"),
                    InlineKeyboardButton.WithCallbackData("❌", "uchir")
                };
                buttons.Add(keycha);
            }
            return buttons;
        }
    }
}

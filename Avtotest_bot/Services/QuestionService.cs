using Avtotest_bot.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Avtotest_bot.Services
{
    class QuestionService
    {
        private List<QuestionModel>? _questions;
        public const int TicketQuestionsCount = 5;
        private ITelegramBotClient _bot;
        public int QuestionsCount
        {
            get
            {
                return _questions!.Count;
            }
        }

        public int ticketsCount
        {
            get
            {
                return QuestionsCount / TicketQuestionsCount;
            }
        }

        public bool SendAnswer(int questionIndex, int choiseIndex)
        {
            return _questions![questionIndex].Choices![choiseIndex].Answer;
        }

        public QuestionService(ITelegramBotClient bot)
        {
            var json = File.ReadAllText("uzlotin.json");
            _questions = JsonConvert.DeserializeObject<List<QuestionModel>>(json);
            _bot = bot;
        }

        public TicketM CreateTicket()
        {
            var random = new Random();
            var ticket = random.Next(0, ticketsCount);

            var StartAnswerIndex = ticket * QuestionService.TicketQuestionsCount;
            var finishAnswerIndex = StartAnswerIndex + QuestionService.TicketQuestionsCount;

            return new TicketM()
            {
                Index = ticket,
                StartIndex = StartAnswerIndex,
                CorrectCount = 0,
                CurrentQuestionIndex = StartAnswerIndex,
                QuestionsCount = QuestionService.TicketQuestionsCount
            };
        }

        InlineKeyboardMarkup CreateQuestionChoiceButtons(int index, int? choiseIndex = null, bool? answer = false)
        {
            var choisesButtons = new List<List<InlineKeyboardButton>>();
            char harf = 'A';
            for (int i = 0; i < _questions![index].Choices!.Count; i++)
            {
                var choiseButtons = new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData($"{harf}", $"{index},{i}")
                };
                choisesButtons.Add(choiseButtons);
                harf ++;
            }
            return new InlineKeyboardMarkup(choisesButtons);
        }

        public void ShowQuestionByIndex(long Id, int index)
        {
            var question = _questions![index];

            var message = $"{question.Id}. {question.Question}\n";
            char harf = 'A';
            for (int i = 0; i < question.Choices!.Count; i++)
            {
                message += $"  {harf}.  {question.Choices[i].Text}\n";
                harf ++;
            }

            try
            {
                var fileBytes = System.IO.File.ReadAllBytes($"Autotest/{question.Media!.Name}.png");
                var ms = new MemoryStream(fileBytes);
                _bot.SendPhotoAsync(chatId: Id, photo: new InputOnlineFile(ms), caption: message, replyMarkup: CreateQuestionChoiceButtons(index));

            }
            catch (Exception)
            {
            }

        }
    }
}

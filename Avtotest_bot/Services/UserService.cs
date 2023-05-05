using System.Reflection.Metadata;
using Avtotest_bot.Models;
using Newtonsoft.Json;

namespace Avtotest_bot.Services
{
    class UserService
    {
        private List<User>? _users;
        private const string UserJsonFilePath = "users.json";
        private readonly QuestionService? _questionService;

        public UserService(QuestionService questionService)
        {
            _questionService = questionService;
            ReadUsersJson();
        }

        public User AddUser(long chatId, string name)
        {
            if (_users!.Any(u => u.ChatId == chatId))
            {
                return _users!.First(u => u.ChatId == chatId);
            }
            else
            {
                var user = new User()
                {
                    ChatId = chatId,
                    Name = name,
                    Tickets = new List<TicketM>()
                };

                for (int i = 0; i < _questionService!.ticketsCount; i++)
                {
                    user.Tickets.Add(new TicketM(i, QuestionService.TicketQuestionsCount));
                }

                _users!.Add(user);

                SaveUserJson();
                return user;
            }
        }

        public void UpdateUserStep(User user, Enum step)
        {
            user.nextMessage = step;
            SaveUserJson();
        }

        private void ReadUsersJson()
        {
            if (File.Exists(UserJsonFilePath))
            {
                var json = File.ReadAllText(UserJsonFilePath);
                _users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            else
            {
                _users = new List<User>();
            }
        }

        public void SaveUserJson()
        {
            var json = JsonConvert.SerializeObject(_users);
            File.WriteAllText(UserJsonFilePath, json);
        }
    }
}

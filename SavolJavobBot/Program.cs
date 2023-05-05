using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using SavolJavobBot.Models; 
int i =0;
var counter = 0;
int hozirgisavol = 0;
int VariantCounter = 0;

var users = new List<User>();
var QUestions = new List<SAvollar>();

var savol1 = new SAvollar()
    {
        Savol = "Hayvonlar qiroli ?",
        Variantlar = new List<string>{"sher", "bo'ri", "tovuq"},
        CorrectAnswer = "sher"
    };
   // .SaveQuestions("Hayvonlar qiroli ?", new List<string>{"sher", "bo'ri", "tovuq"}, "sher");
    QUestions.Add(savol1);
    var savol2 = new SAvollar()
    {
        Savol = "Kim bo'lmoqchisiz ?",
        Variantlar = new List<string>{"bilmayman", "odam bo'lib keldik odam bo'laylik", ".net Developer"},
        CorrectAnswer = ".net Developer"
    };
    //.SaveQuestions("Kim bo'lmoqchisiz ?", new List<string>{"bilmayman", "odam bo'lib keldik odam bo'laylik", ".net Developer"}, ".net Developer");
    QUestions.Add(savol2);
    var savol3 = new SAvollar()
    {
        Savol = "Hozirgi o'qishingizdan qoniqasizmi ?",
        Variantlar = new List<string>{"ha", "yo'q", "ha, biroz", "yo'q, umuman"},
        CorrectAnswer = "yo'q"
    };
    //.SaveQuestions("Hozirgi o'qishingizdan qoniqasizmi ?", new List<string>{"ha", "yo'q", "ha, biroz", "yo'q, umuman"}, "yo'q");
    QUestions.Add(savol3);

string cheklogin = "";
var botManager = new TelegramBotManager();
var bot = botManager.Create("6140972636:AAH4jKyjp_oLTq973WRg2MdNttsuNv9aWEA");
var botDetails = await bot.GetMeAsync();
Console.WriteLine(botDetails.FirstName + " is working..");
botManager.Start(Start);

void Start(Update update)
{
    User user;
    var chatId = update.Message!.From!.Id;
    var ism = update.Message.From.FirstName;
    var xabar = update.Message.Text;

    if (users.Any(u => u.ChatId == chatId))
    {
        user = users.First(u => u.ChatId == chatId);
    }
    else
    {
        user = new User();
        user.ChatId = chatId;
        users.Add(user);
    }
    switch (user.NextMessage)
    {
        case Step.Menu:
        {   
            switch (xabar)
            {
                case "🔐 Sign In": SignIn(user); break;
                case "🔏 Sign Up": SignUp(user); break;
                default:{  bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton2(user));} break;
            }
        } break;
        case Step.SaveLogin: SaveLoginAndSendPasswor(user, xabar!); break;
        case Step.SavePassword: SavePassword(user, xabar!); break;
        case Step.CheckLogin: ChechLoginAndSendPassword(user, xabar!); break;
        case Step.CheckPassword: CheckPassword(user, xabar!); break;
        case Step.KeyingiMenu: ChooseMenu(user, xabar!); break;
        case Step.CheckAnswer: CheckAnswer(user, xabar!); break;
        case Step.AddTest: AddTest2(user, xabar!); break;
        case Step.AddQuestion: SendAnswersCount(user, xabar!); break;
        case Step.Varant: SaveAnswer(user, xabar!); break;
        case Step.CorrectAnswer: SaveCorrectAnswer(user, xabar!); break;
    }
}
void SaveCorrectAnswer(User user, string xabar)
{
    QUestions[counter].CorrectAnswer = xabar;
    bot.SendTextMessageAsync(user.ChatId, "🎉Questioon succesfull added!");
    user.NextMessage = Step.KeyingiMenu;
    bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton(user));

}
void SaveAnswer(User user, string xabar)
{
    QUestions[counter].Variantlar!.Add(xabar);
    Variantlar(user);
}
void Variantlar(User user)
{
    if(i < VariantCounter)
    {
        bot.SendTextMessageAsync(user.ChatId, $"variant {i+1}:");
        user.NextMessage = Step.Varant;
        i ++;
    }
    else
    {
        bot.SendTextMessageAsync(user.ChatId, "Enter correct answer : ");
        user.NextMessage = Step.CorrectAnswer;
        i=0;
    }
}
void SendAnswersCount(User user, string xabar)
{
    VariantCounter = Convert.ToInt32(xabar);
    Variantlar(user);
}
void AddTest2(User user, string xabar)
{
    counter = QUestions.Count;
    var newQuestion = new SAvollar()
    {
        Savol = xabar,
        Variantlar = new List<string>(),
        CorrectAnswer = ""
    };
    QUestions.Add(newQuestion);
    bot.SendTextMessageAsync(user.ChatId, "Nechta variant kiritmoqchisiz : ");
    user.NextMessage = Step.AddQuestion;
}
void SignIn(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "IDENTIFICATION", replyMarkup: new ReplyKeyboardRemove());
    bot.SendTextMessageAsync(user.ChatId, "Enter your login to check:");
    user.NextMessage = Step.CheckLogin;
}
void SignUp(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "", replyMarkup: new ReplyKeyboardRemove());
    bot.SendTextMessageAsync(user.ChatId, "REGISTRATION");
    bot.SendTextMessageAsync(user.ChatId, "Enter your login:");
    user.NextMessage = Step.SaveLogin;
}
void SaveLoginAndSendPasswor(User user, string mesage)
{
    user.Name = mesage;
    bot.SendTextMessageAsync(user.ChatId, "Enter your password:");
    user.NextMessage = Step.SavePassword;
}
void SavePassword(User user, string xabar)
{
    user.Password = xabar;
    bot.SendTextMessageAsync(user.ChatId, "🎉Succesful saved login and parol");
    user.NextMessage = Step.Menu;
    bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton2(user));
    user.UserResults = new List<string>();
}
void ChechLoginAndSendPassword(User user, string xabar)
{
    cheklogin = xabar;
    bot.SendTextMessageAsync(user.ChatId, "Enter your password to check:");
    user.NextMessage = Step.CheckPassword;
}
void CheckPassword(User user, string xabar)
{
    if (user.Password == xabar && cheklogin == user.Name)
    {
        bot.SendTextMessageAsync(user.ChatId, "🔓Welcome");
        user.NextMessage = Step.KeyingiMenu;
        bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton(user));
    }
    else
    {
        bot.SendTextMessageAsync(user.ChatId, "🦠Invalide login or password");
        user.NextMessage = Step.Menu;
        bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton2(user));
    }
}
void ChooseMenu(User user, string xabar)
{
    switch (xabar)
    {
        case "👉Start test": StartTest(user, hozirgisavol); break;
        case "➕Add test": AddTest(user); break;
        case "🙈Results": ShowResults(user); break;
        case "↩️Exit": Exit(user); break;
        default:
        {
        bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton(user));
        user.NextMessage = Step.KeyingiMenu;
        } break;
    }
}
void Exit(User user)
{
    user.NextMessage = Step.Menu;
      bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton2(user));
      user.NextMessage = Step.Menu;
}
void StartTest(User user, int hisob)
{
    string content = "savol: ";
    content += QUestions[hisob].Savol + "\n";
    for (int i = 0; i < QUestions[hisob].Variantlar!.Count; i++)
    {
        content += $"❓" + QUestions[hisob].Variantlar![i] + "\n";
    }
    bot.SendTextMessageAsync(user.ChatId, content, replyMarkup: new ReplyKeyboardRemove());
    user.NextMessage = Step.CheckAnswer;
}
void AddTest(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "🖇Add question ", replyMarkup: new ReplyKeyboardRemove());
    bot.SendTextMessageAsync(user.ChatId, "Savolni kiriting : ");
    user.NextMessage = Step.AddTest;
}
void ShowResults(User user)
{
    bot.SendTextMessageAsync(user.ChatId, "📊Results : ", replyMarkup: new ReplyKeyboardRemove());
    foreach (var usercha in users)
    {
        string satr = string.Empty;
        satr += usercha.Name + "\n";
        foreach (var userResult in usercha.UserResults!)
        {
            satr += userResult + ", ";
        }
        bot.SendTextMessageAsync(user.ChatId, satr);
    }
        bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton(user));
    user.NextMessage = Step.KeyingiMenu;
}
void CheckAnswer(User user, string xabar)
{
    if (xabar == QUestions[hozirgisavol].CorrectAnswer)
    {
        bot.SendTextMessageAsync(user.ChatId, "✅Correct answer");
        user.COrrectCount++;
        hozirgisavol++;
        
        if(hozirgisavol < QUestions.Count)
        {
            StartTest(user, hozirgisavol);
        }
        else
        {
            user.UserResults!.Add($"{user.COrrectCount}/{QUestions.Count}");
            bot.SendTextMessageAsync(user.ChatId, user.UserResults.Last());
            hozirgisavol = 0;
            bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton(user)); 
            user.NextMessage = Step.KeyingiMenu;
            user.COrrectCount = 0;
        }
    }
    else
    {
        bot.SendTextMessageAsync(user.ChatId, "❌Incorrect answer");
        hozirgisavol++;
        if(hozirgisavol < QUestions.Count)
        {
            StartTest(user, hozirgisavol);
        }
        else
        {
            user.UserResults!.Add($"{user.COrrectCount}/{QUestions.Count}");
            bot.SendTextMessageAsync(user.ChatId, user.UserResults.Last());
            hozirgisavol = 0;
            bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: KeyButton(user));
            user.NextMessage = Step.KeyingiMenu;
            user.COrrectCount = 0;
        }
    }
}
ReplyKeyboardMarkup KeyButton(User user)
{
    var keyboard = new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
      {
         new List<KeyboardButton>()
         {
            new KeyboardButton("👉Start test")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("➕Add test")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("🙈Results")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("↩️Exit")
         }
      }
      );
    return keyboard;
}
ReplyKeyboardMarkup KeyButton2(User user)
{
    var keyboard = new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
      {
         new List<KeyboardButton>()
         {
            new KeyboardButton("🔐 Sign In")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("🔏 Sign Up")
         }
      }   
      );
    return keyboard;
}
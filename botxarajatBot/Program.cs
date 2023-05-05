using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using botxarajat.Models;
using botxarajat.Services;
using User = botxarajat.Models.User;


var userservic = new UsersService();
var outlayservic = new OutlaysService();
var roomservic = new RoomsService();

var botManager = new TelegramBotManager();
var bot = botManager.Create("5574249443:AAHDeMnJOMIxSHJkhrXwbHCNXoee-s-e8Uk");
var botDetails = await bot.GetMeAsync();
Console.WriteLine(botDetails.FirstName + " is working ...");
botManager.Start(Start);

void Start(Update update)
{
   User user;
   var chatId = update.Message!.From!.Id;
   var message = update.Message.Text;
   var ism = update.Message.From.FirstName;

   if (userservic.Users.Any(user => user.ChatId == chatId))
   {
      user = userservic.Users.First(user => user.ChatId == chatId);
   }
   else
   {
      user = new User();
      user.ChatId = chatId;
      userservic.Users.Add(user);

   }
   
   switch (user.nextmessage)
   {
      case EnumNextMessage.Menu:
      {
         switch (message)
         {
            case "Sign In": SignIn(user); break;
            case "Sign Up": SignUp(user); break;
            default: bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons3(user)); break;
         }
      } break;
      case EnumNextMessage.Name: SaveNameAndSendPassword(user, message!); break;
      case EnumNextMessage.Parol: SavePasswordAndMenu(user, message!); break;
      case EnumNextMessage.CheckLogin: SendPassword(user, message!); break;
      case EnumNextMessage.CheckPassword: CheckLoginAndPassword(user, message!); break;
      case EnumNextMessage.LeaveMenu: ShowKeyingiMenu(user, message!); break;
      case EnumNextMessage.RoomName: SaveTadbirName(user, message!); break;
      case EnumNextMessage.OxirgiMenu: ShowKeyingidanKeyingiMenu(user, message!); break;
      case EnumNextMessage.ProductNomi: SaveProductNameAndSendProductPrice(user, message!); break;
      case EnumNextMessage.ProductNarx: SaveProductPrice(user, message!); break;
      case EnumNextMessage.CheckKey: ChechTadbirKey(user, message!); break;
   }
}
void ChechTadbirKey(User user, string message)
{
   var rrom = roomservic.tadbirlar.FirstOrDefault(u => u.key == message);
   if(rrom == null)
   {
      bot.SendTextMessageAsync(user.ChatId, "Invalide key, try again");
      bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons(user));
      user.nextmessage = EnumNextMessage.LeaveMenu;
   }
   else
   {
      user.QaysiTadbir = rrom;
      rrom.UsersId!.Add(user.ChatId);
      user.nextmessage = EnumNextMessage.OxirgiMenu;
      bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons2(user));
   }
}
void SaveProductPrice(User user, string message)
{
   var narx = Convert.ToInt64(message);
   user.ManashuTadbirgaPulQoshish!.ProductPrice = narx;
   outlayservic.Save();
   bot.SendTextMessageAsync(user.ChatId, "Xarajat qo'shildi");
   user.nextmessage = EnumNextMessage.OxirgiMenu;
   bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons2(user));
}
void SignIn(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Identifikatsiyadan o'tishingiz lozim!!!");
   bot.SendTextMessageAsync(user.ChatId, "Loginni kiriting:", replyMarkup: new ReplyKeyboardRemove());
   user.nextmessage = EnumNextMessage.CheckLogin;
  
}
void SignUp(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "REGISTRATSIYA");
   bot.SendTextMessageAsync(user.ChatId, "Loginni kiriting: ", replyMarkup: new ReplyKeyboardRemove());
   user.nextmessage = EnumNextMessage.Name;
}
void SaveNameAndSendPassword(User user, string message)
{
   System.Console.WriteLine("=====================>");
   user.SaveLogin(message);
   //users.Add(user);
   userservic.AddUser(user);
   userservic.Save();
   bot.SendTextMessageAsync(user.ChatId, "Parolni kiriting:");
   user.nextmessage = EnumNextMessage.Parol;
}
void SavePasswordAndMenu(User user, string message)
{
   user.Password = message;
   userservic.Users.Add(user);  
   userservic.Save();
   bot.SendTextMessageAsync(user.ChatId, "Login va Parol saqlandi.\n");
   bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons3(user));
   user.nextmessage =EnumNextMessage.Menu;
}
void SendPassword(User user, string message)
{
   bot.SendTextMessageAsync(user.ChatId, "Parolni kiriting:");
   user.nextmessage = EnumNextMessage.CheckPassword;
}
void CheckLoginAndPassword(User user, string message)
{
   UsersService usersService = new UsersService();
   if (usersService.CheckUser(user.Password, user.Ism, user.ChatId))
   {
      bot.SendTextMessageAsync(user.ChatId, "Succesful,  Xush kelibsiz");
      user.nextmessage = EnumNextMessage.LeaveMenu;
      bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons(user));
   }
   else
   {
      bot.SendTextMessageAsync(user.ChatId, "Login yoki parol to'g'ri kelmadi!");
      bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons3(user));
      user.nextmessage = EnumNextMessage.Menu;
   }
}
void ShowKeyingiMenu(User user, string message)
{
   switch (message)
   {
      case "Tadbir yaratish": ShowTadbirYaratish(user); break;
      case "Tadbirga qo'shilish": ShowJoinTadbir(user); break;
      case "Tadbir sozlamalari": ShowKeyTadbir(user); break;
      case "Orqaga": OrqagaQaytish(user); break;
      default:
      {
         user.nextmessage = EnumNextMessage.LeaveMenu;
         bot.SendTextMessageAsync(user.ChatId, "Menyudan tanlang:");
         bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons(user));
         user.nextmessage = EnumNextMessage.LeaveMenu;
      }  break;
   }
}
void ShowTadbirYaratish(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Tadbir nomini kiriting : ", replyMarkup: new ReplyKeyboardRemove());
   user.nextmessage = EnumNextMessage.RoomName;
}
void ShowJoinTadbir(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Tadbir kalitini kiriting: ");
   user.nextmessage = EnumNextMessage.CheckKey;
}
void OrqagaQaytish(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons3(user));
   user.nextmessage = EnumNextMessage.Menu;
}
void SaveTadbirName(User user, string message)
{
   var room = new Room
   {
      RoomName = message,
      RomerChatId = user.ChatId,
      UsersId = new List<long>{user.ChatId},
      Outlays = new List<Xarajat>(),
      key = Guid.NewGuid().ToString("N")
   };
   roomservic.tadbirlar.Add(room);
   user.QaysiTadbir = room;
   //user.QaysiTadbir = room;
   bot.SendTextMessageAsync(user.ChatId, $"Tadbir saqlandi\nTadbir nomi: {message}");
   user.nextmessage = EnumNextMessage.OxirgiMenu;
   bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons2(user));
}
void ShowKeyingidanKeyingiMenu(User user, string message)
{
   switch (message)
   {
      case "Chiqim qilish": ChiqimQilish(user); break;
      case "Chiqimlarni ko'rish": ChiqimlarniKorish(user); break;
      case "Hisob kitob qilish": HisobKitob(user); break;
      case "Orqaga": Exit(user); break;
      default:
      {
         bot.SendTextMessageAsync(user.ChatId, "Menyudan tanlang");

      bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons2(user));
         user.nextmessage = EnumNextMessage.OxirgiMenu;
      }  break;
   }
}
void ChiqimQilish(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Mahsulot nomini kiriting:", replyMarkup: new ReplyKeyboardRemove());
   user.nextmessage = EnumNextMessage.ProductNomi;
}
void ChiqimlarniKorish(User user)
{
   var outlay = "Harajatlar:\n";
   var rows = new List<List<InlineKeyboardButton>>();
 
   foreach (var sarf in user.QaysiTadbir!.Outlays!)
   {
      var inlain0 = InlineKeyboardButton.WithCallbackData(sarf.ProducerChatId.ToString());
      var inlain1 = InlineKeyboardButton.WithCallbackData(sarf.ProductName!);
      var inlain2 = InlineKeyboardButton.WithCallbackData(sarf.ProductPrice.ToString()!);
      rows.Add(new List<InlineKeyboardButton>(){inlain0, inlain1, inlain2});
   }

   var key = new InlineKeyboardMarkup(rows);

   bot.SendTextMessageAsync(user.ChatId, outlay, replyMarkup: key);
}

void HisobKitob(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Hisob - kitob bo'limi");
   var totalPriceSum = user.QaysiTadbir!.Outlays!.Sum(u => u.ProductPrice);

    var averageSum = totalPriceSum / user.QaysiTadbir.UsersId.Count;

    string result = $"\nTotal sum: {totalPriceSum} \nO'rtacha sum {averageSum}\n";

    foreach (var userId in user.QaysiTadbir.UsersId)
    {
        user = userservic.Users.Where(u => u.ChatId == userId).FirstOrDefault()!;

        var userSum = user.QaysiTadbir!.Outlays!.Where(c => c.ProducerChatId == userId).Sum(u => u.ProductPrice);

        result += $"\nUser name:  {user.Ism}, " + $"User sum:  {userSum} " + $"Qoldiq:  {userSum - averageSum}\n";
    }

    bot.SendTextMessageAsync(user.ChatId, result);
}
void Exit(User user)
{
   bot.SendTextMessageAsync(user.ChatId, "Tanlang:", replyMarkup: Buttons(user));
   user.nextmessage = EnumNextMessage.LeaveMenu;
}
void SaveProductNameAndSendProductPrice(User user, string message)
{
   var harajatcha = new Xarajat
   {
      ProducerChatId = user.ChatId,
      ProductName = message,
      SellTime = DateTime.Now
   };
   user.QaysiTadbir!.Outlays!.Add(harajatcha);
   user.ManashuTadbirgaPulQoshish = harajatcha;
   bot.SendTextMessageAsync(user.ChatId, "Mahsulot narxini kiriting: ");
   user.nextmessage = EnumNextMessage.ProductNarx;
}
void ShowKeyTadbir(User user)
{
   bot.SendTextMessageAsync(user.ChatId, $"Tadbir.Key: {user.QaysiTadbir!.key}\nUsers: {user.QaysiTadbir.UsersId!.Count}");
}
ReplyKeyboardMarkup Buttons(User user)
{
      var keyboard = new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
      {
         new List<KeyboardButton>()
         {
            new KeyboardButton("Tadbir yaratish")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Tadbirga qo'shilish")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Tadbir sozlamalari")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Orqaga")
         }
      }
      );
      return keyboard;
}
ReplyKeyboardMarkup Buttons2(User user)
{
      var keyboard = new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
      {
         new List<KeyboardButton>()
         {
            new KeyboardButton("Chiqim qilish")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Chiqimlarni ko'rish")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Hisob kitob qilish")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Orqaga")
         }
      }
      );
      return keyboard;
}
ReplyKeyboardMarkup Buttons3(User user)
{
      var keyboard = new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
      {
         new List<KeyboardButton>()
         {
            new KeyboardButton("Sign In")
         },
         new List<KeyboardButton>()
         {
            new KeyboardButton("Sign Up")
         }
      }
      );
      return keyboard;
}
using botxarajat.Models;
using Newtonsoft.Json;
namespace botxarajat.Services
{
    class UsersService
    {
        public List<User> Users = new List<User>();

        public void Save()
        {
            var ticketString = JsonConvert.SerializeObject(Users);
            File.WriteAllText("users.json", ticketString);
        }

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public bool CheckUser(string password, string username, Int64 chatId)
        {
            string usersString = File.ReadAllText("users.json");
            var users = JsonConvert.DeserializeObject<List<User>>(usersString)!;
            
           var user = users.FirstOrDefault(x => x.ChatId == chatId && x.Password == password && x.Ism == username);

           if(user is null)
           return false;

           return true;
        }
    }
}

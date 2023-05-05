using botxarajat.Models;
using Newtonsoft.Json;

namespace botxarajat.Services
{
    class RoomsService
    {
        public List<Room> tadbirlar = new List<Room>();

        public void Save()
        {
            var roomString = JsonConvert.SerializeObject(tadbirlar);
            File.WriteAllText("rooms.json", roomString);
        }

        public void Read()
        {
            string roomsString = File.ReadAllText("rooms.json");
            User user = JsonConvert.DeserializeObject<User>(roomsString)!;      
        }

        
    }
}

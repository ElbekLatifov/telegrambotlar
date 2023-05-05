using botxarajat.Models;
using Newtonsoft.Json;

namespace botxarajat.Services
{
    class OutlaysService
    {
        public List<Xarajat> outlays = new List<Xarajat>();

        public void Save()
        {
            var outlaytString = JsonConvert.SerializeObject(outlays);
            File.WriteAllText("outlays.json", outlaytString);
        }

        public void Read()
        {
            string outlaysString = File.ReadAllText("outlays.json");
            User user = JsonConvert.DeserializeObject<User>(outlaysString)!;
                
        }
    }
}

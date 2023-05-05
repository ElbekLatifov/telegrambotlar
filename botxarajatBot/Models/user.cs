namespace botxarajat.Models
{
    class User
    {
        public string? Ism;
        public string? Password;
        public Int64 ChatId;
        public EnumNextMessage nextmessage;
        public Room? QaysiTadbir;
        public Xarajat? ManashuTadbirgaPulQoshish;
        public string ShowDannie()
        {
            return $"Foydalanuvchi: {Ism}, Parol: ******, ID: {ChatId}";
        }

        public void SaveLogin(string message)
        {
            Ism = message;
        }

        public bool CheckLoginPassword(string xabar, string parol)
        {
            Console.WriteLine("======================>");
            return Ism == xabar && Password == parol;
        }

    }
}

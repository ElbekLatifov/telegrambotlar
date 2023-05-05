class User
{
    public string? Name;
    public long ChatId;
    public string? Password;
    public int COrrectCount;
    public Step NextMessage;
    public List<string>? UserResults;

    public bool CheckerLogin(string xabar)
    {
        return Name == xabar;
    }
    public bool CheckerPassword(string xabar)
    {
        return Password == xabar;
    }
}
namespace eAccount.Common.Models;

public class ChangeUsernameModel
{
    public Guid Id { get; set; }
    public string Username { get; set; } =  string.Empty;
}
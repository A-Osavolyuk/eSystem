namespace eShop.BlazorWebUI.Models;

public class ChangeUserNameModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; } =  string.Empty;
}
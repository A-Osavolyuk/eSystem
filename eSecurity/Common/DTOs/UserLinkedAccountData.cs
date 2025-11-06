namespace eSecurity.Common.DTOs;

public class UserLinkedAccountData
{
    public bool GoogleConnected { get; set; }
    public bool FacebookConnected { get; set; }
    public bool MicrosoftConnected { get; set; }
    public bool XConnected { get; set; }
    public List<UserLinkedAccountDto> LinkedAccounts { get; set; } = [];
}
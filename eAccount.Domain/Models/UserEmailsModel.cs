namespace eAccount.Domain.Models;

public class UserEmailsModel
{
    public UserEmailDto? PrimaryEmail { get; set; }
    public UserEmailDto? RecoveryEmail { get; set; }
    public List<UserEmailDto> Emails { get; set; } = [];
}
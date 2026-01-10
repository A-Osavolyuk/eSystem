using eSecurity.Core.Common.DTOs;

namespace eSecurity.Client.Common.Models;

public class UserEmailsModel
{
    public UserEmailDto? PrimaryEmail { get; set; }
    public UserEmailDto? RecoveryEmail { get; set; }
    public List<UserEmailDto> Emails { get; set; } = [];
}
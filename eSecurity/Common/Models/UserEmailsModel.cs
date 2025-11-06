using eSecurity.Common.DTOs;
using eSystem.Core.DTOs;

namespace eSecurity.Common.Models;

public class UserEmailsModel
{
    public UserEmailDto? PrimaryEmail { get; set; }
    public UserEmailDto? RecoveryEmail { get; set; }
    public List<UserEmailDto> Emails { get; set; } = [];
}
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Authorization.OAuth.LinkedAccount;

namespace eSecurity.Common.DTOs;

public class UserLinkedAccountDto
{
    public Guid Id { get; set; }
    public LinkedAccountType Type { get; set; }
    public DateTimeOffset? LinkedDate { get; set; }
}